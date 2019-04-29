
using Stormancer.Networking.Processors;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Stormancer
{
    public class P2PTunnelClient : IDisposable
    {
        private ILogger _logger;
        private RequestProcessor _sysCall;
        private Action<P2PTunnelClient, UdpReceiveResult> _onMessageReceived;
        internal UdpClient Client { get; }
        private readonly Task _runTask;

        public ulong PeerId { get; set; }
        public string ServerId { get; set; }        
        public bool IsRunning { get; set; }
        public byte Handle { get; set; }
        public bool ServerSide { get; set; }
        public int HostPort { get; set; }
        

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public P2PTunnelClient(Action<P2PTunnelClient, UdpReceiveResult> onMessageReceived, RequestProcessor sysCall, ILogger logger)
        {
            if(onMessageReceived == null)
            {
                throw new ArgumentNullException(nameof(onMessageReceived));
            }
            _onMessageReceived = onMessageReceived;

            _sysCall = sysCall;
            _logger = logger;
            HostPort = 0;

            Client = new UdpClient(HostPort);
            _runTask = Run(_cts.Token);
        }

        private async Task Run(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                var result = await Client.ReceiveAsync();
                _onMessageReceived(this, result);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _cts.Cancel();
                Client.Dispose();
                try
                {
                    _runTask.Wait();
                }
                catch (System.Exception ex) when(!(ex is ObjectDisposedException))
                {
                    _logger.Error(ex);
                }                

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~P2PTunnelClient()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}