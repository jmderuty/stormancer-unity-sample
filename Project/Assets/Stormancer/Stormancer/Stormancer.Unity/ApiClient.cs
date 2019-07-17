using Stormancer.Client45.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stormancer.Diagnostics;
using System.Net.Http;
using System.Linq;
using System.IO;
using System.Text;

namespace Stormancer
{
    internal class ApiClient
    {
        private ClientConfiguration _config;
        private const string CreateTokenUri = "{0}/{1}/scenes/{2}/token";
        private ILogger _logger;
        private ITokenHandler _tokenHandler;

        public ApiClient(ClientConfiguration configuration, ILogger logger, ITokenHandler tokenHandler)
        {
            _config = configuration;
            _logger = logger;
            _tokenHandler = tokenHandler;
        }

        public async Task<SceneEndpoint> GetSceneEndpoint(List<string> baseUris, string accountId, string applicationName, string sceneId, CancellationToken cancellationToken = default(CancellationToken))
        {

            _logger.Log(Diagnostics.LogLevel.Trace, "APIClient", "Scene endpoint data", $"{accountId};{applicationName};{sceneId}");

            List<string> errors = new List<string>();
            if (baseUris.Count == 0)
            {
                throw new ArgumentException("No server endpoints found in configuration");
            }
            if (_config.EndpointSelectionMode == EndpointSelectionMode.FALLBACK)
            {
                return await GetSceneEndpointImpl(baseUris, errors, accountId, applicationName, sceneId, cancellationToken);
            }
            else if (_config.EndpointSelectionMode == EndpointSelectionMode.RANDOM)
            {
                List<string> baseUris2 = new List<string>();
                Random rand = new Random();
                while (baseUris.Count > 0)
                {
                    int index = rand.Next(baseUris.Count);
                    baseUris2.Add(baseUris[index]);
                    baseUris.RemoveAt(index);
                }
                return await GetSceneEndpointImpl(baseUris2, errors, accountId, applicationName, sceneId, cancellationToken);
            }
            throw new InvalidOperationException("Error selecting server endpoint.");

        }

        private HttpClient CreateHttpClient()
        {
            var result = new HttpClient();

            result.BaseAddress = _config.GetApiEndpoint();
            result.DefaultRequestHeaders.Add("x-version", "3");
            result.DefaultRequestHeaders.Add("Accept", "application/json");

            return result;
        }

        private async Task<SceneEndpoint> GetSceneEndpointImpl(List<string> endpoints, List<string> errors, string accountId, string applicationName, string sceneId, CancellationToken cancellationToken)
        {
            using (var client = CreateHttpClient())
            {

                if (endpoints.Count == 0)
                {
                    string errorMessage = "";
                    foreach (var error in errors)
                    {
                        errorMessage += error;
                    }
                    throw new InvalidOperationException("Failed to connect to the configured server endpoints : " + errorMessage);
                }
                string baseUrl = endpoints[0];
                endpoints.RemoveAt(0);
                var request = CreateRequest("POST", accountId, applicationName, sceneId, _logger);
                HttpResponseMessage response;
                try
                {
                    response = await client.SendAsync(request);
                }
                catch (Exception ex)
                {
                    var message = "Can't reach the server endpoint. " + baseUrl;
                    _logger.Log(Diagnostics.LogLevel.Warn, "APIClient", message, ex.Message);
                    errors.Add($"[{message}:{ex.Message}]");
                    return await GetSceneEndpointImpl(endpoints, errors, accountId, applicationName, sceneId, cancellationToken);
                }
                try
                {
                    var statusCode = (int)response.StatusCode;
                    var message = $"HTTP request on '{baseUrl}' returned status code {statusCode}";
                    _logger.Log(Diagnostics.LogLevel.Trace, "APIClient", message);
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (statusCode >= 200 && statusCode < 300)
                    {
                        if(response.Headers.TryGetValues("x-version", out var values))
                        {
                            var xVersion = values.First();
                            if (xVersion == "2" || xVersion == "3")
                            {
                                _logger.Log(Diagnostics.LogLevel.Trace, "APIClient", "Get token API version : 2");
                                return _tokenHandler.GetSceneEndpointInfos(responseString);
                            }
                        }
                        _logger.Log(Diagnostics.LogLevel.Trace, "APIClient", "Get token API version : 1");
                        return _tokenHandler.DecodeToken(responseString);
                    }
                    else
                    {
                        errors.Add($"[{message} : {statusCode}]");
                        return await GetSceneEndpointImpl(endpoints, errors, accountId, applicationName, sceneId, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Can't get the scene endpoint response: " + ex.Message);
                }
            }

        }

        private HttpRequestMessage CreateRequest(string method, string accountId, string applicationName, string sceneId, ILogger logger)
        {
            var uri = new Uri(_config.GetApiEndpoint(), string.Format(CreateTokenUri, accountId, applicationName, sceneId));
            var message = new HttpRequestMessage(new HttpMethod(method), uri.AbsoluteUri);
            message.Content = new StringContent("", Encoding.UTF8, "application/msgpack");
            logger.Trace("APIClient", "Sending endpoint request to remote server : " + uri);
            return message;
        }

        private async Task<HttpResponseMessage> SendWithRetry(Func<HttpRequestMessage> requestFactory, int firstTry, int secondTry)
        {
            using (var client = CreateHttpClient())
            {


                var request = requestFactory();
                try
                {
                    var firstTryCts = new CancellationTokenSource(firstTry);
                    return await client.SendAsync(request, firstTryCts.Token);
                }
                catch (Exception)
                {
                    _logger.Debug("APIClient", "First call to API timed out.");
                    try
                    {
                        var secondTryCts = new CancellationTokenSource(secondTry);
                        return await client.SendAsync(request, secondTryCts.Token);
                    }
                    catch (Exception)
                    {
                        _logger.Debug("APIClient", "Second call to API timed out.");
                        var thirdTryCts = new CancellationTokenSource(secondTry * 2);

                        return await client.SendAsync(request, thirdTryCts.Token);
                    }
                }
            }
        }

        public async Task<Federation> GetFederation(List<string> endpoints, CancellationToken cancellationToken)
        {
            var errors = new List<string>();
            if (endpoints.Count == 0)
            {
                throw new ArgumentException("No server endpoints found in configuration.");
            }

            if (_config.EndpointSelectionMode == EndpointSelectionMode.FALLBACK)
            {
                var federation = await GetFederationImpl(endpoints, errors, cancellationToken);
                return federation;
            }
            else if (_config.EndpointSelectionMode == EndpointSelectionMode.RANDOM)
            {
                List<string> baseUris = new List<string>();
                Random rand = new Random();
                while (endpoints.Count > 0)
                {
                    int index = rand.Next(endpoints.Count);
                    baseUris.Add(endpoints[index]);
                    endpoints.RemoveAt(index);
                }
                return await GetFederationImpl(baseUris, errors, cancellationToken);
            }
            throw new InvalidOperationException("Error selecting server endpoint.");
        }

        private async Task<Federation> GetFederationImpl(List<string> endpoints, List<string> errors, CancellationToken cancellationToken)
        {
            using (var client = CreateHttpClient())
            {

                if (endpoints.Count == 0)
                {
                    string errorMsg = "";
                    foreach (var error in errors)
                    {
                        errorMsg += error;
                    }
                    throw new InvalidOperationException("Failed to connect to the configured server endpoints : " + errorMsg);
                }

                var baseUri = endpoints[0];
                endpoints.RemoveAt(0);
                var request = new HttpRequestMessage(new HttpMethod("GET"), baseUri + "/_federation");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("x-version", "1.0.0");
                HttpResponseMessage response;
                try
                {
                    response = await client.SendAsync(request);
                }
                catch (Exception ex)
                {
                    var msgStr = $"Can't reach the server endpoint. {baseUri}";
                    _logger.Log(Diagnostics.LogLevel.Warn, "APIClient", msgStr, ex.Message);
                    errors.Add($"[{msgStr} : {ex.Message}]");
                    return await GetFederationImpl(endpoints, errors, cancellationToken);
                }
                try
                {
                    var statusCode = (int)response.StatusCode;
                    var msgStr = $"HTTP request on '{baseUri}' returned status code {statusCode}";
                    var responseStr = await response.Content.ReadAsStringAsync();
                    _logger.Log(Diagnostics.LogLevel.Trace, "APIClient", msgStr);
                    _logger.Log(Diagnostics.LogLevel.Trace, "APIClient", responseStr);
                    if (statusCode >= 200 && statusCode < 300)
                    {
                        _logger.Log(Diagnostics.LogLevel.Trace, "APIClient", "Get token API version 1");
                        return ReadFederationFromJson(responseStr);
                    }
                    else
                    {
                        errors.Add($"[{msgStr} : {statusCode}]");
                        return await GetFederationImpl(endpoints, errors, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Can't get the scene endpoint response: " + ex.Message);
                }
            }
        }

        private Federation ReadFederationFromJson(string json)
        {
            Federation federation = JsonConvert.DeserializeObject<Federation>(json);
            return federation;
        }
    }
}
