using System;

namespace Stormancer
{
    public class P2PTunnel
    {
        private Action _onRelease;

        public string Ip { get; set; }
        public ushort Port { get; set; }
        public P2PTunnelSide Side { get; set; }
        public string Id { get; set; }

        public P2PTunnel(Action onRelease)
        {
            _onRelease = onRelease;
        }

        public void Release()
        {
            _onRelease.Invoke();
        }
    }
}
