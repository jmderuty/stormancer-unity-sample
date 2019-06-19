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
        private P2PService _p2p;
        private Dictionary<string, string> _metadata = new Dictionary<string, string>();

        public Dictionary<string, Route> Routes { get; } = new Dictionary<string, Route>();
        public IConnection Connection { get; }
        public byte Handle { get; }
        public ulong Id => Connection.Id;

        public P2PScenePeer(Scene scene, IConnection connection, P2PService p2p, P2PConnectToSceneMessage message)
        {
            _scene = scene;
            Connection = connection;
            _p2p = p2p;
            Handle = message.SceneHandle;
            _metadata = message.SceneMetadata;
            foreach(var route in message.Routes)
            {
                Routes.Add(route.Name, new Route(route.Name, route.Handle, MessageOriginFilter.Peer, route.Metadata));
            }
            if(connection == null)
            {
                throw new ArgumentNullException("Connection cannot be null");
            }
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public async Task<P2PTunnel> OpenP2PTunnel(string serverId, CancellationToken cancellationToken)
        {
            if(_scene == null)
            {
                throw new InvalidOperationException("Unable to establish P2P tunnel: scene destroyed");
            }
            return await _p2p.OpenTunnel(Connection.Id, _scene.Id + "." + serverId, cancellationToken);

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
            if(!Routes.TryGetValue(routeName, out route))
            {
                throw new InvalidOperationException("The route '" + routeName + "' doesn't exist on the scene");
            }
            var channelUid = Connection.DependencyResolver.Resolve<ChannelUidStore>().GetChannelUid($"P2PScenePeer_{_scene.Id}_{routeName}");
            Connection.SendSystem(stream => 
            {
                stream.WriteByte(Handle);
                writer?.Invoke(stream);
            }, channelUid, priority, reliability);
        }
    }
}