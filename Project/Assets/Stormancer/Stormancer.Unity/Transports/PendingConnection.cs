using Stormancer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
