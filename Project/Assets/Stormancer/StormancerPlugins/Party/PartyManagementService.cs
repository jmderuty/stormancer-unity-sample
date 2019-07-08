using Stormancer.Core;
using Stormancer.Diagnostics;
using System;
using System.Threading.Tasks;

namespace Stormancer.Plugins
{
    public class PartyManagementService
    {
        private ILogger _logger;
        private Scene _scene;

        public PartyManagementService(Scene scene)
        {
            _scene = scene;
            _logger = scene.DependencyResolver.Resolve<ILogger>();
        }

        public async Task<string> CreateParty(PartyRequestDto request)
        {
            return await _scene.RpcTask<PartyRequestDto, string>("partymanagement.createsession", request);
        }
       
    }
}