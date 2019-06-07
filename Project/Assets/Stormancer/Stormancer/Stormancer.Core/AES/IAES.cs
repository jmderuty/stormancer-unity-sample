
using System.IO;

namespace Stormancer
{

    public interface IAES
    {
        ushort IvSize
        {
            get;
        }

        void Encrypt(byte[] data, long dataSize, byte[] iv, long ivSize, Stream outputStream, ulong keyId);

        void Decrypt(byte[] data, long dataSize, byte[] iv, long ivSize, Stream outputStream, ulong keyId);

        byte[] GenerateRandomIV();

        long GetBlockSize();

    }
}
