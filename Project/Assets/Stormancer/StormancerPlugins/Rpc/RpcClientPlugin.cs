﻿using Stormancer.Core;
using Stormancer.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniRx;

namespace Stormancer.Plugins
{
    class RpcClientPlugin : IClientPlugin
    {
        internal const string NextRouteName = "stormancer.rpc.next";
        internal const string ErrorRouteName = "stormancer.rpc.error";
        internal const string CompletedRouteName = "stormancer.rpc.completed";
        internal const string Version = "1.1.0";
        internal const string CancellationRouteName = "stormancer.rpc.cancel";

        internal const string PluginName = "stormancer.plugins.rpc";
        public void Build(PluginBuildContext ctx)
        {
            ctx.RegisterSceneDependencies += (scene, dr) =>
            {
                //var rpcParams = scene.GetHostMetadata(PluginName);

                
                dr.RegisterDependency(new RpcService(scene, true));
            };

            ctx.SceneCreated += scene =>
                {
                    var processor = scene.DependencyResolver.Resolve<RpcService>();

                    scene.AddRoute(NextRouteName, p =>
                    {
                        processor.Next(p);
                    });
                    scene.AddRoute(CancellationRouteName, p =>
                    {
                        processor.Cancel(p);
                    });
                    scene.AddRoute(ErrorRouteName, p =>
                    {
                        processor.Error(p);
                    });
                    scene.AddRoute(CompletedRouteName, p =>
                    {
                        processor.Complete(p);
                    });
                };

            ctx.SceneDisconnected += scene =>
            {
                var processor = scene.DependencyResolver.Resolve<RpcService>();
                processor.Disconnected();
            };
        }

        /// <summary>
        /// Used to send remote procedure call through the RPC plugin
        /// </summary>
        /// <remarks>
        /// If your scene uses the RPC plugin, use `scene.GetService&lt;RpcRequestProcessor&gt;()` to get an instance of this class.
        /// You can also use the `Scene.SendRequest` extension methods.
        /// </remarks>
        public class RpcService
        {

            private ushort _currentRequestId = 0;
            private class Request
            {
                public bool HasCompleted = false;
                public IObserver<Packet<IScenePeer>> Observer { get; set; }
                public TaskCompletionSource<bool> Tcs = new TaskCompletionSource<bool>(); 
            }

            private bool _supportsCancellation;
            private readonly object _lock = new object();
            private readonly ConcurrentDictionary<ushort, Request> _pendingRequests = new ConcurrentDictionary<ushort, Request>();
            private ConcurrentDictionary<Tuple<ulong, ushort>, CancellationTokenSource> _runningRequests = new ConcurrentDictionary<Tuple<ulong, ushort>, CancellationTokenSource>();
            //  private ConcurrentDictionary<long, CancellationTokenSource> _peersCts = new ConcurrentDictionary<long, CancellationTokenSource>();

            private readonly Scene _scene;

            internal RpcService(Scene scene, bool supportsCancellation)
            {
                _scene = scene;
                _supportsCancellation = true;
            }

            internal async Task<TOutput> Rpc<TOutput, TInput>(string route, CancellationToken cancellationToken, TInput[] args)
            {
                var serializer = _scene.DependencyResolver.Resolve<ISerializer>();
                var result = await Rpc(route, (stream) =>
                {
                    foreach(var arg in args)
                    {
                        serializer.Serialize(arg, stream);
                    }
                }, PacketPriority.MEDIUM_PRIORITY);
                return serializer.Deserialize<TOutput>(result.Stream);
            }

            /// <summary>
            /// Starts a RPC to the scene host.
            /// </summary>
            /// <param name="route">The remote route on which the message will be sent.</param>
            /// <param name="writer">The writer used to build the request's content.</param>
            /// <param name="priority">The priority used to send the request.</param>
            /// <returns>An IObservable instance returning the RPC responses.</returns>
            public IObservable<Packet<IScenePeer>> Rpc(string route, Action<Stream> writer, PacketPriority priority)
            {
                return Observable.Create<Packet<IScenePeer>>(
                    observer =>
                    {
                        var rr = _scene.RemoteRoutes.FirstOrDefault(r => r.Name == route);
                        if (rr == null)
                        {
                            throw new ArgumentException("The target route (" + route + ") does not exist on the remote host.");
                        }
                        string version;
                        if (!rr.Metadata.TryGetValue(RpcClientPlugin.PluginName, out version) || version != RpcClientPlugin.Version)
                        {
                            throw new InvalidOperationException("The target remote route (" + route + ") does not support the plugin RPC version " + Version);
                        }

                        var rq = new Request { Observer = observer };
                        var id = this.ReserveId();
                        if (_pendingRequests.TryAdd(id, rq))
                        {

                            _scene.Send(route, s =>
                            {
                                s.Write(BitConverter.GetBytes(id), 0, 2);
                                writer(s);
                            }, priority, PacketReliability.RELIABLE_ORDERED);
                        }

                        return UniRx.Disposable.Create(() =>
                        {
                            Request _;
                            if (!rq.HasCompleted && _pendingRequests.TryRemove(id, out _) && _supportsCancellation)
                            {
                                _scene.Send(CancellationRouteName, s =>
                                {
                                    s.Write(BitConverter.GetBytes(id), 0, 2);
                                });
                            }
                        });
                    });
            }

            /// <summary>
            /// Number of pending RPCs.
            /// </summary>
            public ushort PendingRequests
            {
                get
                {
                    return (ushort)_pendingRequests.Count;
                }
            }

            /// <summary>
            /// Adds a procedure that can be called by remote peer to the scene.
            /// </summary>
            /// <param name="route"></param>
            /// <param name="handler"></param>
            /// <param name="ordered">True if the message should be alwayse receive in order, false otherwise.</param>
            /// <remarks>
            /// The procedure is added to the scene to which this service is attached.
            /// </remarks>
            public void AddProcedure(string route, Func<RequestContext<IScenePeer>, Task> handler, MessageOriginFilter filter, bool ordered)
            {
                this._scene.AddRoute(route, p =>
                {
                    var buffer = new byte[2];
                    p.Stream.Read(buffer, 0, 2);
                    var id = BitConverter.ToUInt16(buffer, 0);
                    var cts = new CancellationTokenSource();
                    var ctx = new RequestContext<IScenePeer>(p.Connection, _scene, id, ordered, new SubStream(p.Stream, false), cts.Token);
                    var identifier = System.Tuple.Create(p.Connection.Id, id);
                    if (_runningRequests.TryAdd(identifier, cts))
                    {
                        handler.InvokeWrapping(ctx).ContinueWith(t =>
                        {
                            _runningRequests.TryRemove(identifier, out cts);
                            if (t.Status == TaskStatus.RanToCompletion)
                            {
                                ctx.SendCompleted();
                            }
                            else if (t.Status == TaskStatus.Faulted)
                            {
                                var errorSent = false;

                                var ex = t.Exception.InnerExceptions.OfType<ClientException>();
                                if (ex.Any())
                                {
                                    ctx.SendError(string.Join("|", ex.Select(e => e.Message).ToArray()));
                                    errorSent = true;
                                }
                                if (t.Exception.InnerExceptions.Any(e => !(e is ClientException)))
                                {
                                    string errorMessage = string.Format("An error occured while executing procedure '{0}'.", route);
                                    if (!errorSent)
                                    {
                                        var errorId = Guid.NewGuid().ToString("N");
                                        ctx.SendError(string.Format("An exception occurred on the remote peer. Error {0}.", errorId));

                                        errorMessage = string.Format("Error {0}. ", errorId) + errorMessage;
                                    }

                                    _scene.DependencyResolver.Resolve<ILogger>().Log(LogLevel.Error, "rpc.server", errorMessage, t.Exception);

                                }

                            }
                        });
                    }
                }, filter, new Dictionary<string, string> { { RpcClientPlugin.PluginName, RpcClientPlugin.Version } });
            }

            private ushort ReserveId()
            {
                lock (this._lock)
                {
                    unchecked
                    {
                        int loop = 0;
                        do
                        {
                            loop++;
                            _currentRequestId++;
                            if (loop > ushort.MaxValue)
                            {
                                throw new InvalidOperationException("Too many requests in progress, unable to start a new one.");
                            }
                        } while (_pendingRequests.ContainsKey(_currentRequestId));
                        return _currentRequestId;
                    }
                }
            }

            private Request GetPendingRequest(Packet<IScenePeer> p)
            {
                ushort id;
                return GetPendingRequest(p, out id);
            }

            private Request GetPendingRequest(Packet<IScenePeer> p, out ushort id)
            {
                id = ExtractRequestId(p);

                Request request;
                if (_pendingRequests.TryGetValue(id, out request))
                {
                    return request;
                }
                else
                {
                    return null;
                }
            }

            private static ushort ExtractRequestId(Packet<IScenePeer> p)
            {
                ushort id;
                var buffer = new byte[2];
                p.Stream.Read(buffer, 0, 2);
                id = BitConverter.ToUInt16(buffer, 0);
                return id;
            }

            internal void Next(Packet<IScenePeer> p)
            {
                var rq = GetPendingRequest(p);
                if (rq != null)
                {
                    rq.Observer.OnNext(p);
                    if (!rq.Tcs.Task.IsCompleted)
                    {
                        rq.Tcs.TrySetResult(true);
                    }
                }
            }

            internal void Error(Packet<IScenePeer> p)
            {
                var id = ExtractRequestId(p);
                Request rq;
                if (_pendingRequests.TryRemove(id, out rq))
                {
                    rq.HasCompleted = true;
                    var message = p.ReadObject<string>();
                    rq.Observer.OnError(new ClientException(message));
                }
            }

            internal async void Complete(Packet<IScenePeer> p)
            {
                var messageSent = p.Stream.ReadByte() != 0;
                ushort id;
                var rq = GetPendingRequest(p, out id);
                Request _;
                if (rq != null)
                {
                    rq.HasCompleted = true;
                    if (messageSent)
                    {
                        await rq.Tcs.Task;
                        _pendingRequests.TryRemove(id, out _);
                        rq.Observer.OnCompleted();
                    }
                    else
                    {
                        _pendingRequests.TryRemove(id, out _);
                        rq.Observer.OnCompleted();
                    }
                }
            }

            internal void Cancel(Packet<IScenePeer> p)
            {
                var buffer = new byte[2];
                p.Stream.Read(buffer, 0, 2);
                var id = BitConverter.ToUInt16(buffer, 0);
                CancellationTokenSource cts;
                if (_runningRequests.TryGetValue(System.Tuple.Create(p.Connection.Id, id), out cts))
                {
                    cts.Cancel();
                }
            }

            internal void Disconnected()
            {
                foreach (var cts in _runningRequests)
                {
                    cts.Value.Cancel();
                }
            }
        }
    }


}
