#if UNITY_EDITOR || UNITY_STANDALONE
using Stormancer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Stormancer.Plugins
{
    public class ConnectionWrapper : IConnection
    {
        private IConnection Connection;
        private StormancerEditorPlugin _plugin;

        public ulong Id { get { return Connection.Id; } }
        public string IpAddress { get { return Connection.IpAddress; } }
        public DateTime ConnectionDate { get { return Connection.ConnectionDate; } }
        public Dictionary<string, string> Metadata { get { return Connection.Metadata; } set{ Connection.Metadata = value; } }
        public string Account { get { return Connection.Account; } }
        public string Application { get { return Connection.Application; } }
        public string Key { get { return Connection.Key; } }

        public Action<string> OnClose { get { return Connection.OnClose; } set { Connection.OnClose = value; } }
        public string CloseReason { get { return Connection.CloseReason; } set { Connection.CloseReason = value; } }
        public int Ping { get { return Connection.Ping; } }
        public IDependencyResolver DependencyResolver => Connection.DependencyResolver;

        public void Close()
        {
            Connection.Close();
        }

        public void SetApplication(string account, string application)
        {
            Connection.SetApplication(account, application);
        }

        public void SendSystem(Action<Stream> writer, int channelUid, PacketPriority priority = PacketPriority.MEDIUM_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED, TransformMetadata transformMetadata = default)
        {
            Connection.SendSystem(writer, channelUid, priority, reliability, transformMetadata);
        }

        public void Close(string reason = "")
        {
            Connection.Close(reason);
        }


        public Task UpdatePeerMetadata()
        {
            return UpdatePeerMetadata(CancellationToken.None);
        }

        public Task UpdatePeerMetadata(CancellationToken token)
        {
            return Connection.UpdatePeerMetadata(token);
        }

        public ConnectionStateCtx GetConnectionState()
        {
            return Connection.GetConnectionState();
        }

        public IObservable<ConnectionStateCtx> GetConnectionStateChangedObservable()
        {
            return Connection.GetConnectionStateChangedObservable();
        }

        public Task SetTimeout(TimeSpan timeout)
        {
            return Connection.SetTimeout(timeout, CancellationToken.None);
        }

        public Task SetTimeout(TimeSpan timeout, CancellationToken token)
        {
            return Connection.SetTimeout(timeout, token);
        }

        public void SendToScene(byte sceneIndex, ushort route, Action<Stream> writer, PacketPriority priority, PacketReliability reliability)
        {
            Connection.SendToScene(sceneIndex, route, writer, priority, reliability);
        }

        public void RegisterComponent<T>(T component)
        {
            Connection.RegisterComponent(component);
        }

        public T Resolve<T>()
        {
            return Connection.Resolve<T>();
        }

        public IConnectionStatistics GetConnectionStatistics()
        {
            return Connection.GetConnectionStatistics();
        }

        public void SetConnectionState(ConnectionStateCtx connectionState)
        {
            Connection.SetConnectionState(connectionState);
        }

        public ConnectionWrapper(IConnection c, StormancerEditorPlugin plugin)
        {
            Connection = c;
            _plugin = plugin;
        }
    }
}
#endif