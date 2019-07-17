using Stormancer.Cluster.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Stormancer.Infrastructure;

namespace Stormancer.Client45.Infrastructure
{
    internal interface ITokenHandler
    {
        // Protocol v1
        SceneEndpoint DecodeToken(string token);

        // Protocol v2
        SceneEndpoint GetSceneEndpointInfos(string token);
    }

    internal class TokenHandler : ITokenHandler
    {
        private readonly ISerializer _tokenSerializer;

        public TokenHandler(ClientConfiguration config)
        {
            _tokenSerializer = config.Serializers.First(s => s.Name == MsgPackSerializer.Instance.Name);
        }

        public SceneEndpoint DecodeToken(string token)
        {
            token = token.Trim('"');
            var data = token.Split('-')[0];
            var buffer = Convert.FromBase64String(data);

            var result = _tokenSerializer.Deserialize<ConnectionData>(new MemoryStream(buffer));

            return new SceneEndpoint { Token = token, TokenData = result };
        }

        public SceneEndpoint GetSceneEndpointInfos(string token)
        {
            var jsonObject = JObject.Parse(token);

            SceneEndpoint sceneEndpoint = new SceneEndpoint();
            sceneEndpoint.Version = 2;
            sceneEndpoint.Token = jsonObject["token"].ToString();
            sceneEndpoint.TokenResponse.Encryption.Algorithm = jsonObject["encryption"]["algorithm"].ToString();
            sceneEndpoint.TokenResponse.Encryption.Key = jsonObject["encryption"]["key"].ToString();
            sceneEndpoint.TokenResponse.Encryption.Mode = jsonObject["encryption"]["mode"].ToString();
            sceneEndpoint.TokenResponse.Encryption.Token = jsonObject["encryption"]["token"].ToString();

            var endpoints = sceneEndpoint.TokenResponse.Endpoints;
            foreach (JProperty transport in jsonObject["endpoints"])
            {
                if (!endpoints.ContainsKey(transport.Name))
                {
                    endpoints.Add(transport.Name, new List<string>());
                }
                sceneEndpoint.TokenResponse.Endpoints[transport.Name].AddRange(transport.Value.AsEnumerable().Select(element => element.ToString()));
               
            }
            sceneEndpoint.TokenData = DecodeToken(sceneEndpoint.Token).TokenData;
            sceneEndpoint.Version = sceneEndpoint.TokenData.Version;
            return sceneEndpoint;
        }
    }
}
