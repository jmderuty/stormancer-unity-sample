
using System;

namespace Stormancer
{

    public struct P2PSessionId
    {

        public byte[] Value { get; set; }

        public bool IsEmpty()
        {
            return Value == null || Value.Length == 0;
        }

        public static P2PSessionId Empty
        {
            get
            {
                return new P2PSessionId();
            }
        }

        public override int GetHashCode()
        {
            if (Value == null)
            {
                return 0;
            }
            int hc = Value.Length;
            for (int i = 0; i < Value.Length; ++i)
            {
                hc = unchecked(hc * 17 + Value[i]);
            }
            return hc;

        }

        public override bool Equals(object obj)
        {
            if (obj is P2PSessionId)
            {
                var other = (P2PSessionId)obj;
                if ((other.Value != null && this.Value == null) || (other.Value == null && this.Value != null))
                {
                    return false;
                }
                if (other.IsEmpty() && this.IsEmpty())
                {
                    return true;
                }
                if (other.Value.Length == this.Value.Length)
                {
                    for (int i = 0; i < this.Value.Length; i++)
                    {
                        if (other.Value[i] != this.Value[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return true;
                }

            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return System.Convert.ToBase64String(Value);
        }

        public static P2PSessionId CreateNew()
        {
            return new P2PSessionId { Value = Guid.NewGuid().ToByteArray() };
        }

        public static P2PSessionId From(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return new P2PSessionId();
            }
            else
            {
                return new P2PSessionId { Value = System.Convert.FromBase64String(sessionId) };
            }
        }
        public static P2PSessionId From(byte[] sessionId)
        {

            if (sessionId == null)
            {
                return new P2PSessionId();
            }
            if (sessionId.Length != 16)
            {
                throw new ArgumentException("Session ids must be 16 bytes long");
            }
            return new P2PSessionId { Value = sessionId };
        }
    }
}