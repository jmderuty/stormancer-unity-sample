using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using System.Threading.Tasks;
using Stormancer.Plugins;
using System.Linq;
using System.Threading;

namespace Stormancer
{

    public static class ClientFactory
    {
        private static ConcurrentDictionary<int, Client> _clients = new ConcurrentDictionary<int, Client>();
        private static ConcurrentDictionary<int, Func<ClientConfiguration>> _configurators = new ConcurrentDictionary<int, Func<ClientConfiguration>>();
        private static SynchronizationContext _mainSynchronizationContext;
        
        public static void SetConfigFactory(int clientId, Func<ClientConfiguration> configurator)
        {
            if(_mainSynchronizationContext == null)
            {
                _mainSynchronizationContext = SynchronizationContext.Current;
            }
            _configurators.TryRemove(clientId, out _);
            _configurators.TryAdd(clientId, configurator);
        }

        public static Client GetClient(int clientId)
        {
            if(!_clients.TryGetValue(clientId, out var client))
            {
                if(!_configurators.TryGetValue(clientId, out var configurator))
                {
                    throw new InvalidOperationException("Missing configuration");
                }
                var config = configurator();
                if(_mainSynchronizationContext != null && !config.IsSynchronizationContextSet)
                {
                    config.SynchronizationContext = _mainSynchronizationContext;
                }
                client = new Client(config);
                _clients.TryAdd(clientId, client);
            }
            return client;
        }

        public static void ReleaseClient(int clientId)
        {
            _clients.TryRemove(clientId, out _);
        }
    }
}