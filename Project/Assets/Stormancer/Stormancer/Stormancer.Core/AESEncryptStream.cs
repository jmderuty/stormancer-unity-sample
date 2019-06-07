
using System.IO;

namespace Stormancer
{
    public class AESEncryptStream
    {
        private IAES _aes;
        private ulong _keyId;

        public AESEncryptStream(IAES aes, ulong keyId)
        {
            _aes = aes;
            _keyId = keyId;
        }

        public byte[] GetBytes()
        {
            if(_aes != null)
            {
                MemoryStream stream = new MemoryStream();
                Encrypt(stream);
                return stream.GetBuffer();
            }
            return new byte[0];
        }

        public void Encrypt(MemoryStream stream)
        {
            if(_aes != null)
            {
                if(stream.Length > 0)
                {
                    var ivSize = _aes.IvSize;
                    var iv = _aes.GenerateRandomIV();

                    _aes.Encrypt(stream.GetBuffer(), stream.Length, iv, ivSize, stream, _keyId);
                }
            }
        }
    }
}
