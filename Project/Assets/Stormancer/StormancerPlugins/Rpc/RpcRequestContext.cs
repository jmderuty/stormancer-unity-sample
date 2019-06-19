﻿using Stormancer.Core;
using System;
using System.IO;
using System.Threading;


namespace Stormancer.Plugins
{

    public class RequestContext<T> where T : IScenePeer
    {
        private Scene _scene;
        private ushort id;
        private bool _ordered;
        private T _peer;
        private bool _msgSent;

        public T RemotePeer
        {
            get
            {
                return _peer;
            }
        }

        public Stream InputStream
        {
            get;
            private set;
        }

        internal RequestContext(T peer, Scene scene, ushort id, bool ordered, Stream stream, CancellationToken token)
        {
            // TODO: Complete member initialization
            this._scene = scene;
            this.id = id;
            this._ordered = ordered;
            this._peer = peer;
            InputStream = stream;
            CancellationToken = token;
        }

        /// <summary>
        /// A Token that gets cancelled if the client cancels the RPC.
        /// </summary>
        public CancellationToken CancellationToken
        {
            get;
            private set;
        }

        private void WriteRequestId(Stream s)
        {
            s.Write(BitConverter.GetBytes(id), 0, 2);
        }
        public void SendValue(Action<Stream> writer, PacketPriority priority)
        {
            if(CancellationToken.IsCancellationRequested)
            {
                return;
            }

            _scene.Send(RpcClientPlugin.NextRouteName, s =>
            {
                WriteRequestId(s);
                writer(s);
            }, priority, this._ordered ? PacketReliability.RELIABLE_ORDERED : PacketReliability.RELIABLE);
            _msgSent = true;
        }


        internal void SendError(string errorMsg)
        {
            if (CancellationToken.IsCancellationRequested)
            {
                return;
            }

            this._scene.Send(RpcClientPlugin.ErrorRouteName, s =>
            {
                WriteRequestId(s);
                _peer.Serializer().Serialize(errorMsg, s);
            }, PacketPriority.MEDIUM_PRIORITY, PacketReliability.RELIABLE);
        }

        internal void SendCompleted()
        {
            if (CancellationToken.IsCancellationRequested)
            {
                return;
            }

            this._scene.Send(RpcClientPlugin.CompletedRouteName, s =>
            {
                s.WriteByte(_msgSent? (byte)1 : (byte)0);

                WriteRequestId(s);
            }, PacketPriority.MEDIUM_PRIORITY, PacketReliability.RELIABLE);
        }
    }
}
