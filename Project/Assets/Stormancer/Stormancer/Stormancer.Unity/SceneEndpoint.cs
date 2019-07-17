using Stormancer.Cluster.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer
{
    public class EncryptionConfiguration
    {
        public string Algorithm { get; set; } = "aes256";
        public string Mode { get; set; } = "GCM";
        public string Key { get; set; }
        public string Token { get; set; }
    }

    public class GetConnectionTokenResponse
    {
        public string Token { get; set; }
        public Dictionary<string, List<string>> Endpoints { get; set; } = new Dictionary<string, List<string>>();
        public EncryptionConfiguration Encryption { get; set; } = new EncryptionConfiguration();
    }

    internal class SceneEndpoint
    {
        public ConnectionData TokenData { get; set; } = new ConnectionData();

        public string Token { get; set; }

        public int Version { get; set; } = 1;

        public GetConnectionTokenResponse TokenResponse { get; set; } = new GetConnectionTokenResponse();
    }
}
