﻿using Stormancer.Core;
using Stormancer.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer.Networking
{
    public class DefaultPacketDispatcher : IPacketDispatcher
    {
        private readonly LazyBool _asyncDispatch;
        public DefaultPacketDispatcher(LazyBool asyncDispatch)
        {
            _asyncDispatch = asyncDispatch;
        }

        private Dictionary<byte, Func<Packet, Task<bool>>> _handlers = new Dictionary<byte, Func<Packet, Task<bool>>>();
        private List<Func<byte, Packet, bool>> _defaultProcessors = new List<Func<byte, Packet, bool>>();

        private async void DispatchImpl(Packet packet)
        {
            bool processed = false;
            int count = 0;
            byte msgType = 0;
            while (!processed && count < 40) // Max 40 layers
            {
                msgType = (byte)packet.Stream.ReadByte();
                if(count > 0)
                {
                    /*//DEBUG
                    if (Enum.IsDefined(typeof(MessageIDTypes), msgType))
                    {
                        UnityEngine.Debug.Log($"Message was in fact a {(MessageIDTypes)msgType}");
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Message was in fact a {(RakNet.DefaultMessageIDTypes)msgType}");
                    }
                    //ENDDEBUG*/
                }
                Func<Packet, Task<bool>> handler;
                if (_handlers.TryGetValue(msgType, out handler))
                {
                    processed = await handler(packet);
                    count++;
                }
                else
                {
                    break;
                }
            }
            foreach (var processor in _defaultProcessors)
            {
                if (processor(msgType, packet))
                {
                    processed = true;
                    break;
                }
            }
            if (!processed)
            {
                using (var memStream = new MemoryStream())
                {
                    memStream.WriteByte(msgType);
                    packet.Stream.CopyTo(memStream);

                    throw new NotSupportedException(string.Format("Couldn't process message. msgId: {0}, content: {1}", msgType, Convert.ToBase64String(memStream.ToArray())));
                }
            }
        }

        public void DispatchPacket(Packet packet)
        {
            if (this._asyncDispatch.Value)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        DispatchImpl(packet);
                    }
                    catch(Exception ex)
                    {
                        MainThread.Post(() => UnityEngine.Debug.LogError(ex));
                    }
                });
            }
            else
            {
                try
                {
                    DispatchImpl(packet);
                }
                catch (Exception ex)
                {
                    MainThread.Post(() => UnityEngine.Debug.LogError(ex));
                }
            }
        }

        public void AddProcessor(IPacketProcessor processor)
        {
            processor.RegisterProcessor(new PacketProcessorConfig(_handlers, _defaultProcessors));
        }


    }
}
