using System.Threading;

namespace Stormancer
{
    class Shutdown
    {
        private CancellationTokenSource _cts;

        private static Shutdown _instance = new Shutdown();
        public static Shutdown Instance
        {
            get
            {
                return _instance;
            }
        }

        private Shutdown()
        {
            _cts = new CancellationTokenSource();
        }

        public CancellationToken GetShutdownToken()
        {
            return _cts.Token;
        }
    }
}