using Stormancer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer
{
    internal class ScenePeer : IScenePeer
    {
        private readonly Scene _scene;
        public ScenePeer(IConnection connection, byte sceneHandle, Dictionary<string, Route> routeMapping, Scene scene)
        {
            Connection = connection;
            Handle = sceneHandle;
            Routes = routeMapping;
            _scene = scene;
        }
        public void Send(string route, Action<System.IO.Stream> writer, PacketPriority priority, PacketReliability reliability)
        {
            if (Connection == null)
            {
                throw new InvalidOperationException("Connection deleted.");
            }

            Route r;
            if (!Routes.TryGetValue(route, out r))
            {
                throw new ArgumentException(string.Format("The route '{0}' is not declared on the server.", route));
            }
            string channelName = $"ScenePeer_{Id}_{route}";
            int channelUid = Connection.DependencyResolver.Resolve<ChannelUidStore>().GetChannelUid(channelName);
            Connection.SendSystem(writer, channelUid, priority, reliability);
        }

        public Dictionary<string, Route> Routes { get; }

        public IConnection Connection { get; }

        public byte Handle { get; }

        public void Disconnect()
        {
            _ = _scene.Disconnect();
        }


        public ulong Id
        {
            get { return Connection.Id; }
        }

        private Dictionary<Type, object> _components = new Dictionary<Type, object>();
    }
}
