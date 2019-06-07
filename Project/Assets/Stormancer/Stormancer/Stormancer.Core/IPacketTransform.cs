using System;
using System.IO;

namespace Stormancer
{

    public interface IPacketTransform 
    {
        void OnSend(Action<Stream> writer, ulong peerId, TransformMetadata metadata = new TransformMetadata());

        void OnReceive(Stream stream, ulong peerId);
    }
}
