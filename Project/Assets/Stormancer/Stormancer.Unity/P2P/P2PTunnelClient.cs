
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
        private Action _onError;
        internal UdpClient Client { get; }
        private readonly Task _runTask;

        public ulong PeerId { get; set; }
        public string ServerId { get; set; }
        public bool IsRunning { get; set; }
        public byte Handle { get; set; }
        public bool ServerSide { get; set; }
        public int HostPort { get; set; }


        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public P2PTunnelClient(Action<P2PTunnelClient, UdpReceiveResult> onMessageReceived, Action onError, ushort tunnelPort, RequestProcessor sysCall, ILogger logger)
        {
            if (onMessageReceived == null)
            {
                throw new ArgumentNullException(nameof(onMessageReceived));
            }
            _onMessageReceived = onMessageReceived;
            _onError = onError;

            _sysCall = sysCall;
            _logger = logger;
            HostPort = 0;

            Client = new UdpClient(tunnelPort);
            _runTask = Run(_cts.Token);
        }

        private async Task Run(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var message = await Client.ReceiveAsync().ConfigureAwait(false);
                    _onMessageReceived(this, message);
                }
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    // something went wrong, clean up!
                    _logger.Log(Diagnostics.LogLevel.Error, "P2PTunnelClient", "An error occurred in the P2PTunnelClient, running the onError callback");
                    _onError?.Invoke();
                }
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
                    _cts.Cancel();
                    Client.Dispose();
                    try
                    {
                        _runTask.Wait();
                    }
                    catch (AggregateException ex) when (ex.InnerException is ObjectDisposedException)
                    {
                    }
                    catch (System.Exception ex)
                    {
                        _logger.Log(Diagnostics.LogLevel.Error, "P2PTunnelClient", "An error occurred in Dispose : " + ex.Message, ex);
                    }

                    disposedValue = true;
                }
            }
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