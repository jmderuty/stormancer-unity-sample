using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Stormancer.Core;

namespace Stormancer
{
    public class P2PScenePeer : IP2PScenePeer
    {
        private Scene _scene;
        private IConnection _connection;
        private P2PService _p2p;
        private byte _handle;
        private Dictionary<string, string> _metadata = new Dictionary<string, string>();
        private Dictionary<string, Route> _remoteRouteMap = new Dictionary<string, Route>();

        public P2PScenePeer(Scene scene, IConnection connection, P2PService p2p, P2PConnectToSceneMessage message)
        {
            _scene = scene;
            _connection = connection;
            _p2p = p2p;
            _handle = message.SceneHandle;
            _metadata = message.Metadata;
            if(connection == null)
            {
                throw new ArgumentNullException("Connection cannot be null");
            }

        }

        public ulong Id => _connection.Id;

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public T GetComponent<T>()
        {
            throw new NotImplementedException();
        }

        public async Task<P2PTunnel> OpenP2PTunnel(string serverId, CancellationToken cancellationToken)
        {
            if(_scene == null)
            {
                throw new InvalidOperationException("Unable to establish P2P tunnel: scene destroyed");
            }
            return await _p2p.OpenTunnel(_connection.Id, _scene.Id + "." + serverId, cancellationToken);

        }

        public Task<P2PTunnel> OpenP2PTunnel(string serverId)
        {
            return OpenP2PTunnel(serverId, CancellationToken.None);
        }

        public void Send(string routeName, Action<Stream> writer, PacketPriority priority, PacketReliability reliability)
        {
            if (_scene == null)
            {
                throw new InvalidOperationException("Unable to send : scene destroyed");
            }
            Route route;
            if(!_remoteRouteMap.TryGetValue(routeName, out route))
            {
                throw new InvalidOperationException("The route '" + routeName + "' doesn't exist on the scene");
            }
            var channelUid = _connection.Resolve<ChannelUidStore>().GetChannelUid($"P2PScenePeer_{_scene.Id}_{routeName}");
            _connection.SendSystem(stream => 
            {
                stream.WriteByte(_handle);
                writer?.Invoke(stream);
            }, channelUid, priority, reliability);
        }
    }
}