using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Stormancer.Networking;
using Stormancer.Core;
using Stormancer.Client45.Infrastructure;
using Stormancer.Diagnostics;
using System.Threading;
using Stormancer.Client45;

namespace Stormancer.Networking.Processors
{
    public class RequestProcessor : IPacketProcessor
    {
        private class Request
        {
            public DateTime LastRefresh;
            public ushort Id;
            //public IObserver<Packet> observer;
            public bool Complete = false;

            private TaskCompletionSource<Packet> _tcs;
            public TaskCompletionSource<Packet> TCS
            {
                get
                {
                    return _tcs;
                }
            }

            public CancellationToken CancellationToken;

            private byte _msgId;
            public byte MsgId
            {
                get
                {
                    return _msgId;
                }
            }

            public Request(byte msgId, TaskCompletionSource<Packet> tcs, CancellationToken ct)
            {
                _msgId = msgId;
                _tcs = tcs;
                CancellationToken = ct;
            }

        }

        public static void Initialize(RequestProcessor processor, IRequestModule[] modules)
        {
            RequestModuleBuilder builder = new RequestModuleBuilder(processor.AddSystemRequestHandler);
            for(int i = 0; i< modules.Length; i++)
            {
                var module = modules[i];
                module.RegisterModule(builder);
            }
        }


        private readonly ConcurrentDictionary<ushort, Request> _pendingRequests = new ConcurrentDictionary<ushort, Request>();
        private readonly ILogger _logger;
        private readonly ISerializer _serializer;

        private bool _isRegistered = false;
        private readonly Dictionary<byte, Func<RequestContext, Task>> _handlers = new Dictionary<byte, Func<RequestContext, Task>>();

        public RequestProcessor(ILogger logger, ISerializer serializer)
        {
            _logger = logger;
            _serializer = serializer;
        }

        public void RegisterProcessor(PacketProcessorConfig config)
        {
            _isRegistered = true;


            config.AddProcessor((byte)MessageIDTypes.ID_SYSTEM_REQUEST, async (packet) =>
            {
                byte sysRequesteID = (byte)packet.Stream.ReadByte();
                RequestContext context = new RequestContext(packet);
                if (!_handlers.ContainsKey(sysRequesteID))
                {
                    context.Error((stream) =>
                    {
                        _serializer.Serialize("No system request handler found.", stream);
                    });
                    return true;
                }

                var handler = _handlers[sysRequesteID];
                try
                {
                    await handler(context);
                }
                catch (Exception ex)
                {
                	if(!context.IsComplete)
                    {
                        context.Error((stream) =>
                        {
                            _serializer.Serialize($"An error occured on the server. {ex.Message}", stream);
                        });
                    }
                    else
                    {
                        _logger.Log(LogLevel.Trace, "RequestProcessor", "An error occurred ", ex.Message);
                    }
                }

                if(!context.IsComplete)
                {
                    context.Complete();
                }

                return true;
            });

            config.AddProcessor((byte)MessageIDTypes.ID_REQUEST_RESPONSE_MSG, async p =>
            {
                var temp = new byte[2];
                p.Stream.Read(temp, 0, 2);
                var id = BitConverter.ToUInt16(temp, 0);
                Request request;
                if (_pendingRequests.TryRemove(id, out request))
                {
                    p.Metadata["request"] = request;
                    request.LastRefresh = DateTime.UtcNow;
                    
                    request.TCS.TrySetResult(p);
                }
                else
                {
                    _logger.Trace("Unknown request id.");
                }

                return true;
            });

            config.AddProcessor((byte)MessageIDTypes.ID_REQUEST_RESPONSE_COMPLETE, async p =>
            {
                var temp = new byte[2];
                p.Stream.Read(temp, 0, 2);
                var id = BitConverter.ToUInt16(temp, 0);
                var hasValues = p.Stream.ReadByte() == 1;
                if (!hasValues)
                {
                    Request request;
                    if (this._pendingRequests.TryRemove(id, out request))
                    {
                        p.Metadata["request"] = request;
                        request.TCS.TrySetResult(null);
                    }
                    else
                    {
                        _logger.Trace("Unknown request id.");
                    }
                }

                return true;
            });

            config.AddProcessor((byte)MessageIDTypes.ID_REQUEST_RESPONSE_ERROR, async p =>
            {
                var temp = new byte[2];
                p.Stream.Read(temp, 0, 2);
                var id = BitConverter.ToUInt16(temp, 0);

                Request request;
                if (_pendingRequests.TryRemove(id, out request))
                {
                    p.Metadata["request"] = request;
                    var serializer = p.Serializer();
                    string msg;
                    if(serializer != null)
                    {
                     msg = serializer.Deserialize<string>(p.Stream);
                    }
                    else
                    {
                        serializer = _serializer;
                        try
                        {
                            msg = serializer.Deserialize<string>(p.Stream);
                        }
                        catch (Exception)
                        {
                            msg = null;
                        }

                        msg = msg ?? "An error occurred on a Stormancer system request, and a serializer could not be found.";
                    }
                    request.TCS.TrySetException(new ClientException(msg));
                    
                }
                else
                {
                    _logger.Trace("Unknown request id.");
                }

                return true;
            });
        }
        


        public Task<Packet> SendSystemRequest(IConnection peer, byte msgId, Action<Stream> writer, PacketPriority priority, CancellationToken ct = default(CancellationToken))
        {
            if(peer != null)
            {
                var tcs = new TaskCompletionSource<Packet>();
                var request = ReserveRequestSlot(msgId, tcs, ct);
                request.CancellationToken = ct;
                if(ct != CancellationToken.None)
                {
                    request.CancellationToken.Register( () => {
                        if(!request.Complete)
                        {
                            _pendingRequests.TryRemove(request.Id, out _);
                            tcs.SetException(new TaskCanceledException());
                        }
                    });
                }

                try
                {
                    TransformMetadata metadata = new TransformMetadata();
                    if(msgId == (byte)SystemRequestIDTypes.ID_SET_METADATA)
                    {
                        metadata.DontEncrypt = true;
                    }
                    peer.SendSystem(stream =>
                    {
                        using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true))
                        {
                            binaryWriter.Write((byte)MessageIDTypes.ID_SYSTEM_REQUEST);
                            binaryWriter.Write(msgId);
                            binaryWriter.Write(request.Id);
                            binaryWriter.Flush();
                            if(writer != null)
                            {
                                writer(stream);
                            }
                        }

                    }, 0, priority, PacketReliability.RELIABLE, metadata);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                return tcs.Task;
            }
            else
            {
                return Task.FromException<Packet>(new ArgumentNullException("peer should not be null"));
            }
        }

        public Task<T1> SendSystemRequest<T1, T2>(IConnection peer, byte msgId, T2 data, CancellationToken ct = default(CancellationToken))
        {
            return SendSystemRequestInternal<T1>(peer, msgId, (stream) => 
            {
                _serializer.Serialize(data, stream);
            }, PacketPriority.MEDIUM_PRIORITY, ct);
        }

        public Task<T> SendSystemRequest<T>(IConnection peer, byte msgId, CancellationToken ct = default(CancellationToken))
        {
            return SendSystemRequestInternal<T>(peer, msgId, (stream) => { }, PacketPriority.MEDIUM_PRIORITY, ct);
        }

        public Task<Packet> SendSystemRequest(IConnection peer, byte msgId, Action<Stream> writer)
        {
            return this.SendSystemRequest(peer, msgId, writer, PacketPriority.MEDIUM_PRIORITY, CancellationToken.None);
        }

        private async Task<TResult> SendSystemRequestInternal<TResult>(IConnection peer, byte msgId, Action<Stream> writer, PacketPriority priority, CancellationToken ct)
        {
            var packet = await SendSystemRequest(peer, msgId, writer, priority, ct);
            return _serializer.Deserialize<TResult>(packet.Stream);
        }

        private Request ReserveRequestSlot(byte msgId, TaskCompletionSource<Packet> tcs, CancellationToken ct)
        {
            Request request = null;
            ushort id = 0;
            while (id < ushort.MaxValue)
            {
                if (!_pendingRequests.TryGetValue(id, out request))
                {
                    request = new Request(msgId, tcs, ct);
                    if (_pendingRequests.TryAdd(id, request))
                    {
                        request.Id = id;
                        break;
                    }
                }
                else
                {
                    string unexpectedMsgId;
                    if (request != null)
                    {
                        unexpectedMsgId = "msgId: " + _pendingRequests[id].MsgId;
                    }
                    else
                    {
                        unexpectedMsgId = " request is null";
                    }
                    _logger.Log(LogLevel.Warn, "RequestProcessor", "Unexpected occupied request slot " + id, unexpectedMsgId);
                }
                id++;
            }

            if(request != null)
            {
                return request;
            }
            _logger.Error("Unable to create a new request: Too many pending requests.");
            throw new InvalidOperationException("Unable to create new request: Too many pending requests.");
        }

        public void AddSystemRequestHandler(byte msgId, Func<RequestContext, Task> handler)
        {
            if (_isRegistered)
            {
                throw new InvalidOperationException("Can only add handler before 'RegisterProcessor' is called.");
            }
            _handlers.Add(msgId, handler);
        }
    }


    public class RequestContext
    {
        private Packet _packet;
        private byte[] _requestId;
        private MemoryStream _stream = new MemoryStream();
        private bool _didSendValues = false;

        public Packet Packet
        {
            get
            {
                return _packet;
            }
        }

        public RequestContext(Packet p)
        {
            this._packet = p;
            IsComplete = false;
            _requestId = new byte[2];
            p.Stream.Read(_requestId, 0, 2);

            p.Stream.CopyTo(_stream);
            _stream.Seek(0, SeekOrigin.Begin);
        }

        public Stream InputStream
        {
            get
            {
                return _stream;
            }
        }

        public bool IsComplete
        {
            get;
            private set;
        }

        public void Send(Action<Stream> writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (IsComplete)
            {
                throw new InvalidOperationException("The request is already completed.");
            }
            this._didSendValues = true;
            _packet.Connection.SendSystem(s =>
            {
                s.WriteByte((byte)MessageIDTypes.ID_REQUEST_RESPONSE_MSG);
                s.Write(_requestId, 0, 2);
                writer(s);
            }, 0);
        }

        public void Complete()
        {
            _packet.Connection.SendSystem(s =>
            {
                s.WriteByte((byte)MessageIDTypes.ID_REQUEST_RESPONSE_COMPLETE);
                s.Write(_requestId, 0, 2);

                s.WriteByte((byte)(_didSendValues ? 1 : 0));
            }, 0);
        }

        public void Error(Action<Stream> writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            _packet.Connection.SendSystem(s =>
            {
                s.WriteByte((byte)MessageIDTypes.ID_REQUEST_RESPONSE_ERROR);
                s.Write(_requestId, 0, 2);
                writer(s);
            }, 0);
        }

    }
}
