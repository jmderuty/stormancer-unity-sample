using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Stormancer.Core
{
    public class ConnectionStateCtx
    {
        public ConnectionState State { get; set; }
        public string Reason { get; set; }
        public ConnectionStateCtx(ConnectionState state, string reason = "")
        {
            State = state;
            Reason = reason;
        }

    }

    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected, 
        Disconnecting
    }

    public interface IConnection
    {
        /// <summary>
        /// Sends a system msg to the remote peer.
        /// </summary>
        /// <param  name="writer">A function to write in the stream.</param>
        /// <param  name="priority">The priority of the message.</param>
        /// <param  name="reliability">The reliability of the message.</param>
        void SendSystem(Action<Stream> writer, int channelUid, PacketPriority priority = PacketPriority.MEDIUM_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED, TransformMetadata transformMetadata = new TransformMetadata());

        /// <summary>
        /// Sends a packet to the target remote scene.
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="route"></param>
        /// <param name="writer"></param>
        /// <param name="priority"></param>
        /// <param name="reliability"></param>
        /// <param name="channel"></param>
        void SendToScene(byte sceneIndex,
            ushort route,
            Action<Stream> writer,
            PacketPriority priority,
            PacketReliability reliability);

        /// <summary>
        /// Set the account id and the application name.
        /// </summary>
        /// <param name="account">The account id.</param>
        /// <param name="application">The application name.</param>
        void SetApplication(string account, string application);

        /// <summary>
        /// Close the connection.
        /// </summary>
        /// <param name="reason">The reason of the connection closing.</param>
        void Close(string reason = "");

        /// <summary>
        /// Ip address of the remote peer.
        /// </summary>
        string IpAddress { get; }

        /// <summary>
        /// The connection's Ping in milliseconds.
        /// </summary>
        int Ping { get; }

        string Key { get; }

        IDependencyResolver DependencyResolver { get; }

        string CloseReason { get; set; }

        /// <summary>
        /// Unique id in the node for the connection.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        /// Returns the connection date.
        /// </summary>
        DateTime ConnectionDate { get; }

        /// <summary>
        /// Account of the application which the peer is connected to.
        /// </summary>
        string Account { get; }

        /// <summary>
        /// Name of the application to which the peer is connected.
        /// </summary>
        string Application { get; }

        /// <summary>
        /// Metadata associated with the connection.
        /// </summary>
        Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Register components.
        /// </summary>
        void RegisterComponent<T>(T component);

        /// <summary>
        /// Gets a service from the object.
        /// </summary>
        /// <typeparam name="T">Type of the service to fetch.</typeparam>
        /// <param name="key">A string containing the service key.</param>
        /// <returns>A service object.</returns>
        T Resolve<T>();

        IConnectionStatistics GetConnectionStatistics();

        Task UpdatePeerMetadata();

        Task UpdatePeerMetadata(CancellationToken token);

        ConnectionStateCtx GetConnectionState();

        void SetConnectionState(ConnectionStateCtx connectionState);

        IObservable<ConnectionStateCtx> GetConnectionStateChangedObservable();

        Task SetTimeout(TimeSpan timeout);

        Task SetTimeout(TimeSpan timeout, CancellationToken token);

        /// <summary>
        /// Event fired when the connection has been closed.
        /// </summary>
        Action<string> OnClose { get; set; }
    }
}
