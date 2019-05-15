using Stormancer.Core;
using Stormancer.Plugins;
using System.Threading;
using System.Threading.Tasks;

namespace Stormancer.Networking
{
    public class PendingConnection
    {

        public string Endpoint { get; set; }
        public TaskCompletionSource<IConnection> Tcs { get; set; } = new TaskCompletionSource<IConnection>();
        public CancellationToken CancellationToken { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }

        public bool IsP2P => ParentId != "";
    }
}
