using Stormancer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stormancer.Networking
{
    /// <summary>
    /// Manages connections
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// Generates an unique connection id for this node.
        /// </summary>
        /// <returns>A `long` containing an unique id.</returns>
        /// <remarks>Only used on servers.</remarks>
        long GenerateNewConnectionId();


        Task<IConnection> AddPendingConnection(ulong id);

        /// <summary>
        /// Adds a connection to the manager
        /// </summary>
        /// <param name="connection">The connection object to add.</param>
        /// <remarks>This method is called by the infrastructure when a new connection connects to a transport.</remarks>
        void NewConnection(IConnection connection);

        /// <summary>
        /// Closes the target connection.
        /// </summary>
        /// <param name="connection">The connection to close.</param>
        /// <param name="reason">The reason of the closure.</param>
        void CloseConnection(IConnection connection,string reason);

        /// <summary>
        /// Returns a connection by id. Returns null if the connection is not completed
        /// </summary>
        /// <param name="id">The connection Id</param>
        /// <returns></returns>
        IConnection GetConnection(ulong id);

        /// <summary>
        /// Returns a connection by id. Returns null if the connection is not completed
        /// </summary>
        /// <param name="id">The connection Id</param>
        /// <returns></returns>
        IConnection GetConnection(string id);


        /// <summary>
        /// Returns a connection by id, of create a new if not already created
        /// </summary>
        /// <param name="id">The connection Id</param>
        /// <param name="connectionFactory">A factory to create a new connection if not existing</param>
        /// <returns></returns>
        Task<IConnection> GetConnection(string id, Func<string, Task<IConnection>> connectionFactory);

        /// <summary>
        /// Returns the connection count
        /// </summary>
        /// <returns></returns>
        int GetConnectionCount();

        /// <summary>
        /// Close All Connections
        /// </summary>
        /// <param name="reason">The reason of the closure.</param>
        /// <returns></returns>
        void CloseAllConnections(string reason);

        /// <summary>
        /// Set a timeout duration on all the current open connections
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        void SetTimeout(TimeSpan timeout, CancellationToken ct);
    }
}
