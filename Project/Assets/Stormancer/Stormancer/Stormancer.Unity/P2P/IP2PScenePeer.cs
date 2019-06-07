
using Stormancer.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Stormancer
{
    public interface IP2PScenePeer : IScenePeer
    {
        Task<P2PTunnel> OpenP2PTunnel(string serverId, CancellationToken ct);

        Task<P2PTunnel> OpenP2PTunnel(string serverId);
    }
}
