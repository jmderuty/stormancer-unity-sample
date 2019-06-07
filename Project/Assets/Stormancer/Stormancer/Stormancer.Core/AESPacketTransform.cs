
using Stormancer.Networking;
using System;
using System.IO;

namespace Stormancer
{

    public class AESPacketTransform : IPacketTransform
    {
        private IAES _aes;
        private bool _enabled;

        public AESPacketTransform(IAES aes, ClientConfiguration config)
        {
            _aes = aes;
            _enabled = config.EncryptionEnabled;
        }
        public void OnSend(Action<Stream> writer, ulong peerId, TransformMetadata metadata = default)
        {
            if (_enabled && !metadata.DontEncrypt)
            {
                var writerCopy = writer;
                writer = (stream) =>
                {
                    stream.WriteByte((byte)MessageIDTypes.ID_ENCRYPTED);
                    AESEncryptStream aesStream = new AESEncryptStream(_aes, peerId);
                };
            }

        }

        public void OnReceive(Stream stream, ulong peerId)
        {

        }
    }
}
