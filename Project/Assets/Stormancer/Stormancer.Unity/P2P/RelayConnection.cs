using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Stormancer.Core;
using Stormancer.Networking;

namespace Stormancer
{
    public class RelayConnection : IConnection
    {
        private IConnection _serverConnection;
        private Dictionary<string, string> _metadata = new Dictionary<string, string>();
        private P2PSessionId _p2pSessionId;
        private ulong _remotePeerId;
        private string _ipAddress;
        private DateTime _connectionDate = DateTime.Now;
        private IDependencyResolver _dependencyResolver;
        private ISerializer _serializer;

        public string IpAddress => _ipAddress;

        public int Ping => _serverConnection.Ping;

        public string Key => _p2pSessionId.ToString();

        public IDependencyResolver DependencyResolver => _dependencyResolver;

        public ulong Id => _remotePeerId;

        public DateTime ConnectionDate => _connectionDate;

        public string Account => _serverConnection.Account;

        public string Application => _serverConnection.Application;

        public Dictionary<string, string> Metadata { get => _metadata; set => _metadata = value; }
        public Action<string> OnClose { get; set; }

        public RelayConnection(IConnection serverConnection, string address, ulong remotePeerId, P2PSessionId p2pSessionId, ISerializer serializer)
        {
            _serverConnection = serverConnection;
            _p2pSessionId = p2pSessionId;
            _remotePeerId = remotePeerId;
            _ipAddress = address;
            _serializer = serializer;
        }

        public void Close(string reason = "")
        {

        }

        public ConnectionStateCtx GetConnectionState()
        {
            return _serverConnection.GetConnectionState();
        }

        public IObservable<ConnectionStateCtx> GetConnectionStateChangedObservable()
        {
            return _serverConnection.GetConnectionStateChangedObservable();
        }

        public IConnectionStatistics GetConnectionStatistics()
        {
            return _serverConnection.GetConnectionStatistics();
        }

        public void SendSystem(Action<Stream> writer, int channelUid, PacketPriority priority = PacketPriority.MEDIUM_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED, TransformMetadata transformMetadata = default)
        {
            _serverConnection.SendSystem(stream =>
            {
                stream.WriteByte((byte)MessageIDTypes.ID_P2P_RELAY);
                if(_serializer != null)
                {
                    _serializer.Serialize(_p2pSessionId.Value, stream);
                    _serializer.Serialize((byte)priority, stream);
                    _serializer.Serialize((byte)reliability, stream);
                    writer?.Invoke(stream);
                }
            }, channelUid, priority, reliability);
        }

        public void SetApplication(string account, string application)
        {
            throw new NotImplementedException();
        }

        public void SetConnectionState(ConnectionStateCtx connectionState)
        {
            throw new NotImplementedException();
        }

        public async Task SetTimeout(TimeSpan timeout)
        {
            await SetTimeout(timeout, CancellationToken.None);
        }

        public Task SetTimeout(TimeSpan timeout, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task UpdatePeerMetadata()
        {
            await UpdatePeerMetadata(CancellationToken.None);
        }

        public Task UpdatePeerMetadata(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
