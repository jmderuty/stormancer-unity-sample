using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;

namespace Stormancer.Core
{
    /// <summary>
    /// Represents a Stormancer scene.
    /// </summary>
    /// <remarks>
    /// In a Stormancer application, users connect to scenes to interact with each other and the application. 
    /// A scene has 2 faces: A scene host, currently only server side scene hosts are supported, and scene clients.
    /// </remarks>
    public interface IScene
    {

        /// <summary>
        /// Disconnect from the scene.
        /// /// </summary> 
        Task Disconnect();

        /// <summary>
        /// Add a route to the scene.
        /// /// </summary> 
        /// <param name="routeName">Route name.</param>
		/// <param name="handler">Function which handle the receiving messages from the server on the route.</param>
        /// <param name="metadata">Metadatas about the Route.</param>
		/// Add a route for each different message type.
        void AddRoute(string routeName, Action<Packet<IScenePeer>> handler, MessageOriginFilter filter, Dictionary<string, string> metadata);


        /// <summary>
        /// Send a packet to a route.
        /// /// </summary> 
        /// <param name="peerFilter">Peer receiver.</param>
        /// <param name="routeName">Route name.</param>
        /// <param name="writer">Function where we write the data in the byte stream.</param>
        /// <param name="priority">Message priority on the network.</param>
        /// <param name="reliability">Message reliability behavior.</param>
        void Send(PeerFilter peerFilter, string routeName, Action<Stream> writer, PacketPriority priority = PacketPriority.MEDIUM_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED, string channelIdentifier = "");

        /// <summary>
        /// Send a packet to a route.
        /// /// </summary> 
		/// <param name="routeName">Route name.</param>
		/// <param name="writer">Function where we write the data in the byte stream.</param>
		/// <param name="priority">Message priority on the network.</param>
		/// <param name="reliability">Message reliability behavior.</param>
        void Send(string routeName, Action<Stream> writer, PacketPriority priority = PacketPriority.MEDIUM_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED, string channelIdentifier = "");
        
        /// <summary>
        /// Returns the connection state to the the scene.
        /// /// </summary> 
		ConnectionState GetCurrentConnectionState();

        Subject<ConnectionStateCtx> SceneConnectionStateObservable { get; }

        /// <summary>
        /// Returns the scene id.
        /// /// </summary>
        string Id { get; }

        /// <summary>
        /// Returns a host metadata value.
        /// </summary>
        string GetHostMetadata(string key);

        /// <summary>
        /// Returns a host connection.
        /// </summary>
        IConnection HostConnection { get; }

        /// <summary>
        /// Returns a copy of the local routes.
        /// </summary>
        Route[] LocalRoutes { get; }
        
        /// <summary>
        /// Returns a copy of the remote routes.
        /// </summary>
        Route[] RemoteRoutes { get; }

        /// <summary>
        /// Creates an IObservable<Packet> instance that listen to events on the specified route.
        /// </summary>
        /// <param "routeName">A string containing the name of the route to listen to.</param>
        /// <returns>An IObservable<Packet> instance that fires each time a message is received on the route.</returns>
        IObservable<Packet<IScenePeer>> OnMessage(string routeName, MessageOriginFilter filter, Dictionary<string, string> metadata);

        /// <summary>
        /// Get an array containing the scene host connections.
        /// </summary>
        /// <returns>An array containing the scene host connections.</returns>
        IScenePeer[] RemotePeers { get; }

        /// <summary>
        /// Returns the peer connection to the host.
        /// </summary>
        IScenePeer Host { get; }

        IDependencyResolver DependencyResolver { get; }

        /// <summary>
        /// Fire when a packet is received in the scene. 
        /// </summary>
        Action<Packet> OnPacketReceived { get; }

        /// <summary>
        /// True if the instance is an host. False if it's a client.
        /// </summary>
        bool IsHost { get; }

        P2PTunnel RegisterP2PServer(string p2pServerId);

        Task<IP2PScenePeer> OpenP2PConnection(string p2pToken, CancellationToken ct);

        Task<IP2PScenePeer> OpenP2PConnection(string p2pToken);

        Dictionary<string, string> GetSceneMetadata();
    }

   
}
