using Stormancer.Client45;
using Stormancer.Networking;
using Stormancer.Networking.Processors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Stormancer
{
    public class P2PTunnels
    {
        private ISerializer _serializer;
        private ILogger _logger;
        private RequestProcessor _sysCall;
        private ClientConfiguration _configuration;
        private IConnectionManager _connections;
        private Dictionary<string, ServerDescriptor> _servers = new Dictionary<string, ServerDescriptor>();
        private ConcurrentDictionary<(ulong, byte), P2PTunnelClient> _tunnels = new ConcurrentDictionary<(ulong, byte), P2PTunnelClient>();

        public P2PTunnels(RequestProcessor sysCall, IConnectionManager connections, ISerializer serializer, ClientConfiguration configuration, ILogger logger)
        {
            _sysCall = sysCall;
            _connections = connections;
            _configuration = configuration;
            _serializer = serializer;
            _logger = logger;
        }

        public P2PTunnel CreateServer(string serverId, P2PTunnels tunnels)
        {
            var tunnel = new P2PTunnel(() =>
            {
                _ = tunnels.DestroyServer(serverId);
            });
            tunnel.Id = serverId;
            tunnel.Ip = "127.0.0.1";
            tunnel.Port = _configuration.ServerGamePort;
            tunnel.Side = P2PTunnelSide.Host;
            ServerDescriptor descriptor = new ServerDescriptor();
            descriptor.Id = tunnel.Id;
            descriptor.HostName = tunnel.Ip;
            descriptor.Port = tunnel.Port;
            _servers[descriptor.Id] = descriptor;
            return tunnel;
        }

        public async Task<P2PTunnel> OpenTunnel(ulong connectionId, string serverId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = _connections.GetConnection(connectionId);
            if(connection == null)
            {
                throw new InvalidOperationException("No p2p connection established to the target peer");
            }
            _logger.Log(Diagnostics.LogLevel.Debug, "P2PTunnel", $"Open Tunnel to {connection.ToString()}");
            OpenTunnelResult result = await _sysCall.SendSystemRequest<OpenTunnelResult,string>(connection, (byte)SystemRequestIDTypes.ID_P2P_OPEN_TUNNEL, serverId, cancellationToken);

            if(result.UseTunnel)
            {
                var client = new P2PTunnelClient((tunnelClient, msg) =>
                {
                    OnMessageReceived(tunnelClient, msg);
                },
                () => { // on error

                    _ = _sysCall.SendSystemRequest(connection, (byte)SystemRequestIDTypes.ID_P2P_CLOSE_TUNNEL, stream => { stream.WriteByte(result.Handle); }, Core.PacketPriority.HIGH_PRIORITY, cancellationToken);
                },
                _configuration.TunnelPort, _sysCall, _logger);
                client.Handle = result.Handle;
                client.PeerId = connectionId;
                client.ServerId = serverId;
                client.ServerSide = false;

                _tunnels.TryAdd((connectionId, result.Handle), client);
                var tunnel = new P2PTunnel(() =>
                {
                    _ = DestroyTunnel(connectionId, result.Handle);
                });
                tunnel.Id = serverId;
                tunnel.Ip = "127.0.0.1";
                tunnel.Port = (ushort)((IPEndPoint)client.Client.Client.LocalEndPoint).Port;
                return tunnel;
            }
            else
            {
                var tunnel = new P2PTunnel(() => { });
                var endpointSplit = result.Endpoint.Split(':');
                tunnel.Id = serverId;
                tunnel.Ip = endpointSplit[0];
                tunnel.Port = Convert.ToUInt16(endpointSplit[1]);
                return tunnel;
            }
        }

        public byte AddClient(string serverId, ulong clientPeerId)
        {
            ServerDescriptor server;
            if(!_servers.TryGetValue(serverId, out server))
            {
                throw new InvalidOperationException($"The server with id {serverId} does not exist");
            }
            for (byte handle = 0; handle < 255; handle++)
            {
                var key = (clientPeerId, handle);
                if(!_tunnels.ContainsKey(key))
                {
                    var client = new P2PTunnelClient((tunnelClient, message) => 
                    {
                        OnMessageReceived(tunnelClient, message);
                    },
                    () =>
                    { // on error
                        var connection = _connections.GetConnection(serverId);
                        _ = _sysCall.SendSystemRequest(connection, (byte)SystemRequestIDTypes.ID_P2P_CLOSE_TUNNEL, stream => { stream.WriteByte(handle); }, Core.PacketPriority.HIGH_PRIORITY);
                    }, server.Port, _sysCall, _logger);
                    client.Handle = handle;
                    client.PeerId = clientPeerId;
                    client.ServerId = serverId;
                    client.ServerSide = true;
                    client.HostPort = server.Port;
                    _tunnels.TryAdd(key, client);
                    return handle;
                }
            }
            throw new InvalidOperationException("Unable to create tunnel handle : Too many tunnels opened between the peers.");
        }

        public void CloseTunnel(byte handle, ulong peerId)
        {
            P2PTunnelClient tunnelClient;
            if(_tunnels.TryGetValue((peerId, handle), out tunnelClient))
            {
                tunnelClient.Dispose();
            }
        }

        public async Task DestroyServer(string serverId)
        {
            List<(ulong, byte)> itemsToDelete = new List<(ulong, byte)>();
            foreach(var tunnel in _tunnels)
            {
                if(tunnel.Value.ServerId == serverId && tunnel.Value.ServerSide)
                {
                    var connection = _connections.GetConnection(tunnel.Value.PeerId);
                    itemsToDelete.Add(tunnel.Key);
                    if(connection != null)
                    {
                        await _sysCall.SendSystemRequest<bool, byte>(connection, (byte)SystemRequestIDTypes.ID_P2P_CLOSE_TUNNEL, tunnel.Key.Item2); 
                    }
                }
            }
            foreach(var item in itemsToDelete)
            {
                P2PTunnelClient tunnelClient;
                _tunnels.TryRemove(item, out tunnelClient);
                tunnelClient.Dispose();
            }
        }

        public async Task DestroyTunnel(ulong peerId, byte handle)
        {
            P2PTunnelClient client;
            if (_tunnels.TryRemove((peerId, handle), out client))
            {
                client.Dispose();
                var connection = _connections.GetConnection(peerId);
                if(connection != null)
                {
                    await _sysCall.SendSystemRequest<bool, byte>(connection, (byte)SystemRequestIDTypes.ID_P2P_CLOSE_TUNNEL, handle);
                }
            }
        }

        public void ReceivedFrom(ulong id, Stream stream)
        {
            byte handle = (byte)stream.ReadByte();
            var buffer = new byte[1464];
            var read = stream.Read(buffer, 0, (int)stream.Length - 1);
            P2PTunnelClient client;
            if(_tunnels.TryGetValue((id, handle), out client))
            {
                if(client != null)
                {
                    UnityEngine.Debug.Log($"sending message to UDP port {client.HostPort}");
                    client.Client.Send(buffer, read, new IPEndPoint(IPAddress.Parse("127.0.0.1"), client.HostPort));
                }
                else
                {
                    _tunnels.TryRemove((id, handle), out client);
                }
            }
        }

        public void OnMessageReceived(P2PTunnelClient client, UdpReceiveResult message)
        {
            var connection = _connections.GetConnection(client.PeerId);
            if(connection != null)
            {
                if(client.HostPort == 0)
                {
                    client.HostPort = message.RemoteEndPoint.Port;
                }
                int channelUid = connection.DependencyResolver.Resolve<ChannelUidStore>().GetChannelUid("P2PTunnels_" + connection.Id);
                connection.SendSystem(stream =>
                {
                    stream.WriteByte((byte)MessageIDTypes.ID_P2P_TUNNEL);
                    stream.WriteByte(client.Handle);
                    stream.Write(message.Buffer, 0, message.Buffer.Length);
                }, channelUid, Core.PacketPriority.IMMEDIATE_PRIORITY, Core.PacketReliability.UNRELIABLE);
            }
        }
    }
}
