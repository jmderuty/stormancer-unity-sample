using System;
using System.Collections.Generic;

namespace Stormancer
{

    public class KeyStore
    {

        private Dictionary<ulong, byte[]> _keys = new Dictionary<ulong, byte[]>();
        public Dictionary<ulong, byte[]> Keys
        {
            get => _keys;
        }

        public byte[] GetKey(ulong keyId)
        {
            byte[] ret;
            if(_keys.TryGetValue(keyId, out ret))
            {
                return ret;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Key not found");
            }
        }

        public void DeleteKey(ulong keyId)
        {
            _keys.Remove(keyId);
        }


    }
}
