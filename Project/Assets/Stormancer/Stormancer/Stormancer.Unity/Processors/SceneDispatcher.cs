using Stormancer.Core;
using Stormancer.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer.Processors
{
    internal class SceneDispatcher : IPacketProcessor
    {
        private ConcurrentDictionary<byte, ConcurrentQueue<Packet>> _waitingPackets = new ConcurrentDictionary<byte, ConcurrentQueue<Packet>>();

        public void RegisterProcessor(PacketProcessorConfig config)
        {
            config.AddCatchAllProcessor(Handler);
        }

        private byte GetSceneIndex(byte sceneHandle)
        {
            return (byte)(sceneHandle - MessageIDTypes.ID_SCENES);
        }

        private List<Scene> getHandles(IConnection connection)
        {
            return connection.DependencyResolver.Resolve<List<Scene>>();
        }

        public void AddScene(IConnection connection, Scene scene)
        {
            if(connection != null && scene != null)
            {
                var handles = getHandles(connection);
                if(handles != null)
                {
                    handles.Insert(scene.Host.Handle - (byte)MessageIDTypes.ID_SCENES, scene);
                    ConcurrentQueue<Packet> waitingPackets;
                    if (_waitingPackets.TryRemove(scene.Host.Handle, out waitingPackets))
                    {
                        Packet packet;
                        while (waitingPackets.TryDequeue(out packet))
                        {
                            packet.Metadata["scene"] = scene;
                            scene.HandleMessage(packet);
                        }
                    }
                }
            }
        }

        public void RemoveScene(IConnection connection, byte sceneHandle)
        {
            if (connection != null)
            {
                var handles = getHandles(connection);
                if (handles != null)
                {
                    handles.Insert(sceneHandle - (byte)MessageIDTypes.ID_SCENES, null);
                }
            }
        }

        private bool Handler(byte sceneHandle, Packet packet)
        {
            if (sceneHandle < (byte)MessageIDTypes.ID_SCENES)
            {
                return false;
            }
            var handles = getHandles(packet.Connection);
            var scene = handles.ElementAtOrDefault(sceneHandle - (byte)MessageIDTypes.ID_SCENES);
            if (scene == null)
            {
                var queue = _waitingPackets.GetOrAdd(sceneHandle, handle => new ConcurrentQueue<Packet>());
                queue.Enqueue(packet);
                return true;
            }
            else
            {
                packet.Metadata["scene"] = scene;
                MainThread.Post(() =>
                {
                    scene.HandleMessage(packet);
                });

                return true;
            }
        }

        public Scene GetScene(IConnection connection, byte sceneHandle)
        {
            var handles = getHandles(connection);
            var index = GetSceneIndex(sceneHandle);
            if (index < handles.Count)
            {
                return handles.ElementAt(index);
            }
            else
            {
                return null;
            }
        }

    }
}
