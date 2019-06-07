using System.IO;

namespace Stormancer
{
    enum NTStatus : long
    {
        STATUS_UNSUCCESSFUL = 0xC0000001L,
        STATUS_SUCCESS = 0x00000000L,
        STATUS_AUTH_TAG_MISMATCH = 0xC000A002L,
        STATUS_BUFFER_TOO_SMALL = 0xC0000023L,
        STATUS_INVALID_BUFFER_SIZE = 0xC0000206L,
        STATUS_INVALID_HANDLE = 0xC0000008L,
        STATUS_INVALID_PARAMETER = 0xC000000DL,
        STATUS_NOT_SUPPORTED = 0xC00000BBL,
        STATUS_DATA_ERROR = 0xC000003E

    }

    public class AESWindows : IAES
    {
        private KeyStore _key;

        public AESWindows(KeyStore keyStore)
        {
            _key = keyStore;
        }

        public void Encrypt(byte[] data, long dataSize, byte[] iv, long ivSize, Stream outputStream, ulong keyId)
        {
            InitAES(keyId);
        }

        public ushort IvSize => throw new System.NotImplementedException();

        public void Decrypt(byte[] data, long dataSize, byte[] iv, long ivSize, Stream outputStream, ulong keyId)
        {
            throw new System.NotImplementedException();
        }


        public byte[] GenerateRandomIV()
        {
            throw new System.NotImplementedException();
        }

        public long GetBlockSize()
        {
            throw new System.NotImplementedException();
        }

        private bool InitAES(ulong keyId)
        {
            throw new System.NotImplementedException();
        }

        private void CleanAES()
        {
            throw new System.NotImplementedException();
        }

    }
}
