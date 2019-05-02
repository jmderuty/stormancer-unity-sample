using RakNet;
using Stormancer.Core;
using Stormancer.Diagnostics;
using Stormancer.Infrastructure;
using Stormancer.Networking.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using Stormancer.Client45;

namespace Stormancer.Networking
{

    internal class RakNetConnection : IConnection
    {
        private class RakNetConnectionStatistics : IConnectionStatistics
        {
            public static IConnectionStatistics GetConnectionStatistics(RakNetConnection connection)
            {
                var result = new RakNetConnectionStatistics();
                using (var stats = connection._rakPeer.GetStatistics(connection._rakPeer.GetSystemAddressFromGuid(connection._guid)))
                {
                    result.PacketLossRate = stats.packetlossLastSecond;
                    result.BytesPerSecondLimitationType = stats.isLimitedByOutgoingBandwidthLimit ? BPSLimitationType.OutgoingBandwidth : (stats.isLimitedByCongestionControl ? BPSLimitationType.CongestionControl : BPSLimitationType.None);
                    result.BytesPerSecondLimit = (long)(stats.isLimitedByOutgoingBandwidthLimit ? stats.BPSLimitByOutgoingBandwidthLimit : stats.BPSLimitByCongestionControl);
                    result._queuedBytes = stats.bytesInSendBuffer.ToArray();
                    result._queuedPackets = stats.messageInSendBuffer.ToArray();
                }

                return result;
            }

            private RakNetConnectionStatistics()
            {
            }

            public float PacketLossRate { get; private set; }

            public BPSLimitationType BytesPerSecondLimitationType { get; private set; }

            public long BytesPerSecondLimit { get; private set; }

            private double[] _queuedBytes;
            public double QueuedBytes
            {
                get
                {
                    return this._queuedBytes.Sum();
                }
            }

            public double QueuedBytesForPriority(Core.PacketPriority priority)
            {
                return this._queuedBytes[(int)priority];
            }

            private uint[] _queuedPackets;

            public int QueuedPackets
            {
                get { return this._queuedPackets.Cast<int>().Sum(); }
            }

            public int QueuedPacketsForPriority(Core.PacketPriority priority)
            {
                return (int)(this._queuedPackets[(int)priority]);
            }
        }



        private RakPeerInterface _rakPeer;
        private RakNetGUID _guid;

        private readonly Action<string> _closeAction;
        private Dictionary<string, string> _metadata = new Dictionary<string, string>();
        private ILogger _logger;
        private ConnectionStateCtx _connectionState;
        private Subject<ConnectionStateCtx> _connectionStateObservable;
        private IDependencyResolver _dependencyResolver;
        private string _key;

        internal RakNetConnection(RakNetGUID guid, ulong id, string key, RakPeerInterface peer, ILogger logger, IDependencyResolver resolver)
        {
            ConnectionDate = DateTime.UtcNow;
            LastActivityDate = DateTime.UtcNow;
            Id = id;
            _key = key;
            _guid = guid;
            _dependencyResolver = resolver;
            _rakPeer = peer;
            _logger = logger;
            _connectionStateObservable = new Subject<ConnectionStateCtx>();
            Action<ConnectionStateCtx> onNext = (state) =>
            {
                _connectionState = state;
            };

            Action<Exception> onError = (exception) =>
            {
                _logger.Log(LogLevel.Error, "RakNetConnection", "Connection state change failed", exception.Message);
            };
            _connectionStateObservable.Subscribe(onNext, onError);

            RegisterComponent<ChannelUidStore>(new ChannelUidStore());
        }

        /// <summary>
        /// Id of the connection
        /// </summary>
        public RakNetGUID Guid
        {
            get
            {
                return _guid;
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
        }

        /// <summary>
        /// The reason of the closure
        /// </summary>
        public string CloseReason { get; set; }

        /// <summary>
        /// Last activity 
        /// </summary>
        public DateTime LastActivityDate { get; internal set; }


        /// <summary>
        /// IP address of the connection.
        /// </summary>
        public string IpAddress
        {
            get
            {
                return _rakPeer.GetSystemAddressFromGuid(_guid).ToString();
            }
        }

        /// <summary>
        /// Connection metadata
        /// </summary>
        public Dictionary<string, string> Metadata
        {
            get => _metadata;
            set
            {
                _metadata = value;
            }
        }
        public Task UpdatePeerMetadata()
        {
            return UpdatePeerMetadata(CancellationToken.None);
        }

        public async Task UpdatePeerMetadata(CancellationToken token)
        {
            var requestProcessor = _dependencyResolver.Resolve<RequestProcessor>();
            token = CancellationTokenHelpers.CreateLinkedShutdownToken(token);
            var packet = await requestProcessor.SendSystemRequest(this, (byte)SystemRequestIDTypes.ID_SET_METADATA, (stream) =>
            {
                _dependencyResolver.Resolve<ISerializer>().Serialize(Metadata, stream);
            }, Core.PacketPriority.MEDIUM_PRIORITY, token);
            _logger.Log(LogLevel.Trace, "RakNetConnection::updatePeerMetadata", "Updated peer metadata");
        }


        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close(string reason)
        {
            _logger.Log(LogLevel.Trace, "RakNetConnection", $"Closing connection {reason}", Id);
            if(_connectionState.State == Core.ConnectionState.Connected || _connectionState.State == Core.ConnectionState.Connecting)
            {
                SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Disconnecting, reason));
                _closeAction?.Invoke(reason);
                SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Disconnected, reason));
            }
        }


        public int Ping
        {
            get
            {
                if(_rakPeer == null)
                {
                    return _rakPeer.GetLastPing(_guid);
                }
                else
                {
                    return -1;
                }
            }
        }

        public void SendSystem(Action<System.IO.Stream> writer, int channelUid, Core.PacketPriority priority = Core.PacketPriority.MEDIUM_PRIORITY, Core.PacketReliability reliability = Core.PacketReliability.RELIABLE_ORDERED, TransformMetadata metadata = new TransformMetadata())
        {
            if(_metadata["type"] != "p2p")
            {
                //var packetTransform = new AESPacketTransform(Resolve<IAES>(), Resolve<ClientConfiguration>());
                //packetTransform.OnSend(writer, Id, metadata);
            }
            MemoryStream stream = new MemoryStream();
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            else
            {
                writer(stream);
            }
            stream.Flush();
            var dataSize = stream.Position;

            if(_rakPeer != null)
            {
                int orderingChannel = channelUid % 16;
                var result = _rakPeer.Send(stream.ToArray(), (int)dataSize, (RakNet.PacketPriority)priority, (RakNet.PacketReliability)reliability, (char)orderingChannel, _guid, false);
                if(result == 0)
                {
                    throw new InvalidOperationException("Raknet failed to send the message.");
                }
            }
            else
            {
                throw new InvalidOperationException("RakPeer has been destroyed.");
            }
        }

        public void SetApplication(string account, string application)
        {
            if (this.Account == null)
            {
                this.Account = account;
                this.Application = application;
            }
        }

        public ulong Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Connection date
        /// </summary>
        public DateTime ConnectionDate { get; internal set; }

        /// <summary>
        /// The account id of the application to which this connection is connected.
        /// </summary>
        public string Account { get; private set; }

        /// <summary>
        /// The id of the application to which this connection is connected.
        /// </summary>
        public string Application { get; private set; }

        public ConnectionStateCtx GetConnectionState()
        {
            return _connectionState;
        }

        public IObservable<ConnectionStateCtx> GetConnectionStateChangedObservable()
        {
            return _connectionStateObservable;
        }



        public void SetConnectionState(ConnectionStateCtx connectionState)
        {
            if (_connectionState != connectionState)
            {
                _connectionState = connectionState;
                _connectionStateObservable.OnNext(connectionState);
                if (connectionState.State == Core.ConnectionState.Disconnected)
                {
                    _connectionStateObservable.OnCompleted();
                }
            }
        }
        public Task SetTimeout(TimeSpan timeout)
        {
            return SetTimeout(timeout, CancellationToken.None);
        }

        public Task SetTimeout(TimeSpan timeout, CancellationToken ct)
        {
            var totalMS = timeout.TotalMilliseconds;
            var totalMSStr = totalMS.ToString();
            Metadata["timeout"] = totalMSStr;
            if(_rakPeer != null)
            {
                _rakPeer.SetTimeoutTime((uint)totalMS, _rakPeer.GetSystemAddressFromGuid(_guid));
            }
            else
            {
                return Task.FromException(new NullReferenceException("peer is null"));
            }
            return UpdatePeerMetadata(ct);
        }

        /// <summary>
        /// Returns an hashcode based on Id.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Equals implementation for IConnection
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var v2 = obj as IConnection;
            if (v2 == null)
            {
                return false;
            }
            else
            {
                return v2.Id == this.Id;
            }
        }
        public void SendRaw(Action<Stream> writer, Stormancer.Core.PacketPriority priority, Stormancer.Core.PacketReliability reliability, char channel)
        {

            var stream = new BitStream();
            writer(new BSStream(stream));
            var result = _rakPeer.Send(stream, (RakNet.PacketPriority)priority, (RakNet.PacketReliability)reliability, channel, this.Guid, false);

            if (result == 0)
            {
                throw new InvalidOperationException("Failed to send message.");
            }
        }

        /// <summary>
        /// Sends a scene request to the remote peer.
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="route"></param>
        /// <param name="writer"></param>
        /// <param name="priority"></param>
        /// <param name="reliability"></param>
        /// <param name="channel"></param>
        public void SendToScene(byte sceneIndex, ushort route, Action<System.IO.Stream> writer, Stormancer.Core.PacketPriority priority, Stormancer.Core.PacketReliability reliability)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            var stream = new BitStream();
            var s = new BSStream(stream);
            s.WriteByte(sceneIndex);
            s.Write(BitConverter.GetBytes(route), 0, 2);
            writer(s);
            var result = _rakPeer.Send(stream, (RakNet.PacketPriority)priority, (RakNet.PacketReliability)reliability, (char)0, this.Guid, false);

            if (result == 0)
            {
                throw new InvalidOperationException("Failed to send message.");
            }
        }


        public Action<string> OnClose
        {
            get;
            set;
        }

        private Dictionary<Type, object> _localData = new Dictionary<Type, object>();


        public T Resolve<T>()
        {
            object result;
            if (_localData.TryGetValue(typeof(T), out result))
            {
                return (T)result;
            }
            else
            {
                return default(T);
            }
        }

        public void RegisterComponent<T>(T component)
        {
            _localData.Add(typeof(T), component);
        }

        public IConnectionStatistics GetConnectionStatistics()
        {
            return RakNetConnectionStatistics.GetConnectionStatistics(this);
        }
    }
}
