
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Stormancer.Plugins
{
    class RandomAuthenticationProvider : IAuthenticationProvider
    {
        public void Initialize()
        {
            
        }

        // TODO : random == Steam ?
        public Task<AuthParameters> GetAuthArgs()
        {
            AuthParameters auth = new AuthParameters();
            
            Random rand = new Random((int)DateTime.Now.Ticks);
            string ticket = "user"+ rand.Next();
            auth.Type = "steam";
            auth.Parameters = new Dictionary<string, string> { { "ticket", ticket } };
            return Task.FromResult(auth);
        }
    }
}
