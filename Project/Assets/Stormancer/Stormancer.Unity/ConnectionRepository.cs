using Stormancer.Core;
using Stormancer.Networking;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Stormancer.Diagnostics;

namespace Stormancer
{
    public class ConnectionRepository : IConnectionManager
    {
        private class PendingConnection
        {
            public ulong Id;
            public TaskCompletionSource<IConnection> Tcs;
        }

        private ConcurrentDictionary<string, IConnection> _connectionsByKey = new ConcurrentDictionary<string, IConnection>();
        private ConcurrentDictionary<ulong, IConnection> _connections = new ConcurrentDictionary<ulong, IConnection>();
        private ConcurrentDictionary<ulong, PendingConnection> _pendingP2PConnections = new ConcurrentDictionary<ulong, PendingConnection>();
        private ILogger _logger;

        public ConnectionRepository(ILogger logger)
        {
            _logger = logger;
        }

        public long GenerateNewConnectionId()
        {
            throw new NotImplementedException();
        }

        public Task<IConnection> AddPendingConnection(ulong id)
        {
            var tcs = new TaskCompletionSource<IConnection>();
            if(_pendingP2PConnections.TryAdd(id, new PendingConnection { Id = id, Tcs = tcs }))
            {
                _logger.Log(LogLevel.Info, "P2P", "Added pending connection from id ", id);
                return tcs.Task;
            }
            else
            {
                return Task.FromException<IConnection>(new Exception($"Connection with id {id} already exists"));
            }
        }

        public void NewConnection(IConnection connection)
        {
            _logger.Log(LogLevel.Trace, "Connections", $"Adding connection {connection.IpAddress}", connection.Id);

            connection.OnClose += (reason) =>
            {
                _connections.TryRemove(connection.Id, out _);
                _connectionsByKey.TryRemove(connection.Key, out _);
            };

            _connections.TryAdd(connection.Id, connection);
            _connectionsByKey.TryAdd(connection.Key, connection);

            _logger.Log(LogLevel.Info, "Connections", "Completed connection", connection.Id);
            PendingConnection pendingConnection;
            if(_pendingP2PConnections.TryRemove(connection.Id, out pendingConnection))
            {
                pendingConnection.Tcs.SetResult(connection);
            }

        }

        public void CloseConnection(IConnection connection, string reason)
        {
            if(connection != null)
            {   _connections.TryRemove(connection.Id, out _);
                _connectionsByKey.TryRemove(connection.Key, out _);
                connection.Close(reason);
            }
        }

        public IConnection GetConnection(ulong id)
        {
            IConnection connection;
            if (_connections.TryGetValue(id, out connection))
            {
                return connection;
            }
            return null;
        }

        public IConnection GetConnection(string id)
        {
            IConnection connection;
            if(_connectionsByKey.TryGetValue(id, out connection))
            {
                return connection;
            }
            return null;
        }

        public async Task<IConnection> GetConnection(string id, Func<string, Task<IConnection>> connectionFactory)
        {
            IConnection connection;
            if (_connectionsByKey.TryGetValue(id, out connection))
            {
                return connection;
            }
            else
            {
                try
                {
                    var factoryResultTask = connectionFactory(id);
                    connection = await factoryResultTask;
                    var pId = connection.Id;
                    var key = connection.Key;
                    connection.OnClose += (reason) =>
                    {
                        _connections.TryRemove(pId, out _);
                        _connectionsByKey.TryRemove(key, out _);
                    };
                    _connections.TryAdd(pId, connection);
                    _connectionsByKey.TryAdd(id, connection);
                    return connection;
                }
                catch (System.Exception ex)
                {
                    _connectionsByKey.TryRemove(id, out _);
                    _logger.Error(ex);
                    throw ex;
                }
            }
        }

        public int GetConnectionCount()
        {
            return _connections.Count;
        }

        public void CloseAllConnections(string reason)
        {
            foreach(var connection in _connections)
            {
                connection.Value.Close(reason);
            }
        }

        public void SetTimeout(TimeSpan timeout, CancellationToken ct)
        {
            foreach (var connection in _connections)
            {
                connection.Value.SetTimeout(timeout, ct);
            }
        }
    }
}
