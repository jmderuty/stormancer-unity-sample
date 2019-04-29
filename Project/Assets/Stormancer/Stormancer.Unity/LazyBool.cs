using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stormancer
{
    public class LazyBool
    {
        private bool _value;
        private Func<bool> _factory;

        public bool IsValueCreated { get; private set; }

        private object _lockRoot = new object();
        public bool Value
        {
           get {
                if (!IsValueCreated)
                {
                    lock (_lockRoot)
                    {
                        if (!IsValueCreated)
                        {
                            _value = _factory();
                            IsValueCreated = true;
                        }
                    }
                }
                return _value;
            } 
        }

        public LazyBool(Func<bool> factory)
        {
            _factory = factory;
        }
    }
}