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
using Stormancer.Plugins;

namespace Stormancer.Networking
{

    internal class RakNetConnection : IConnection
    {
        private class RakNetConnectionStatistics : IConnectionStatistics
        {
            public static IConnectionStatistics GetConnectionStatistics(RakNetConnection connection)
            {
                var result = new RakNetConnectionStatistics();
                using (var raknetGuid = new RakNetGUID(connection.RaknetGuid))
                {
                    using (var stats = connection._rakPeer.GetStatistics(connection._rakPeer.GetSystemAddressFromGuid(raknetGuid)))
                    {
                        result.PacketLossRate = stats.packetlossLastSecond;
                        result.BytesPerSecondLimitationType = stats.isLimitedByOutgoingBandwidthLimit ? BPSLimitationType.OutgoingBandwidth : (stats.isLimitedByCongestionControl ? BPSLimitationType.CongestionControl : BPSLimitationType.None);
                        result.BytesPerSecondLimit = (long)(stats.isLimitedByOutgoingBandwidthLimit ? stats.BPSLimitByOutgoingBandwidthLimit : stats.BPSLimitByCongestionControl);
                        result._queuedBytes = stats.bytesInSendBuffer.ToArray();
                        result._queuedPackets = stats.messageInSendBuffer.ToArray();
                    }
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



        private readonly RakPeerInterface _rakPeer;

        private Dictionary<string, string> _metadata = new Dictionary<string, string>();
        private ILogger _logger;
        private ConnectionStateCtx _connectionState;
        private readonly Subject<ConnectionStateCtx> _connectionStateObservable;
        public Guid UniqueId { get; }

        internal RakNetConnection(RakNetGUID guid, ulong id, string key, RakPeerInterface peer, ILogger logger, IDependencyResolver resolver)
        {
            UniqueId = System.Guid.NewGuid();
            ConnectionDate = DateTime.UtcNow;
            LastActivityDate = DateTime.UtcNow;
            Id = id;
            Key = key;
            RaknetGuid = guid.g;
            DependencyResolver = resolver;
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

            DependencyResolver.RegisterDependency<ChannelUidStore>(new ChannelUidStore());
        }

        /// <summary>
        /// Id of the connection
        /// </summary>
        public ulong RaknetGuid { get; }

        public string Key { get; }

        public IDependencyResolver DependencyResolver { get; }

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
                using (var rakGuid = new RakNetGUID(RaknetGuid))
                {
                    return _rakPeer.GetSystemAddressFromGuid(rakGuid).ToString();
                }
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
            var requestProcessor = DependencyResolver.Resolve<RequestProcessor>();
            token = CancellationTokenHelpers.CreateLinkedShutdownToken(token);
            var packet = await requestProcessor.SendSystemRequest(this, (byte)SystemRequestIDTypes.ID_SET_METADATA, (stream) =>
            {
                DependencyResolver.Resolve<ISerializer>().Serialize(Metadata, stream);
            }, Core.PacketPriority.MEDIUM_PRIORITY, token);
            _logger.Log(LogLevel.Trace, "RakNetConnection::updatePeerMetadata", "Updated peer metadata");
        }


        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close(string reason)
        {
            _logger.Log(LogLevel.Trace, "RakNetConnection", $"Closing connection {reason}", Id);
            if (_connectionState.State == Core.ConnectionState.Connected || _connectionState.State == Core.ConnectionState.Connecting)
            {
                SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Disconnecting, reason));
                OnClose?.Invoke(reason);
                SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Disconnected, reason));
            }
        }


        public int Ping
        {
            get
            {
                if (_rakPeer == null)
                {
                    using (var rakGuid = new RakNetGUID(RaknetGuid))
                    {
                        return _rakPeer.GetLastPing(rakGuid);
                    }
                }
                else
                {
                    return -1;
                }
            }
        }

        public void SendSystem(Action<System.IO.Stream> writer, int channelUid, Core.PacketPriority priority = Core.PacketPriority.MEDIUM_PRIORITY, Core.PacketReliability reliability = Core.PacketReliability.RELIABLE_ORDERED, TransformMetadata metadata = new TransformMetadata())
        {
            if (_metadata["type"] != "p2p")
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

            if (_rakPeer != null)
            {
                int orderingChannel = channelUid % 16;
                using (var rakGuid = new RakNetGUID(RaknetGuid))
                {
                    var result = _rakPeer.Send(stream.ToArray(), (int)dataSize, (RakNet.PacketPriority)priority, (RakNet.PacketReliability)reliability, (char)orderingChannel, rakGuid, false);

                    if (result == 0)
                    {
                        throw new InvalidOperationException("Raknet failed to send the message.");
                    }
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

        public ulong Id { get; }

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
            if (_rakPeer != null)
            {
                using (var rakGuid = new RakNetGUID(RaknetGuid))
                {
                    _rakPeer.SetTimeoutTime((uint)totalMS, _rakPeer.GetSystemAddressFromGuid(rakGuid));
                }
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

            using (var stream = new BitStream())
            using (var bsStream = new BSStream(stream))
            {
                {
                    writer(bsStream);
                    using (var rakGuid = new RakNetGUID(RaknetGuid))
                    {
                        var result = _rakPeer.Send(stream, (RakNet.PacketPriority)priority, (RakNet.PacketReliability)reliability, channel, rakGuid, false);

                        if (result == 0)
                        {
                            throw new InvalidOperationException("Failed to send message.");
                        }
                    }
                }
            }

        }

        public Action<string> OnClose
        {
            get;
            set;
        }

        private Dictionary<Type, object> _localData = new Dictionary<Type, object>();


        public IConnectionStatistics GetConnectionStatistics()
        {
            return RakNetConnectionStatistics.GetConnectionStatistics(this);
        }

        public override string ToString()
        {
            return $"Address {IpAddress}, id {Id}, guid {RaknetGuid}, unique Id {UniqueId}";
        }
    }
}
