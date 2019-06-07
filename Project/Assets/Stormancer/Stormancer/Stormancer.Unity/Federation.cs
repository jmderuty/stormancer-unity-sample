using System;
using System.Collections.Generic;

namespace Stormancer
{
    public class FederationCluster
    {
        public string Id { get; set; }
        public List<string> Endpoints { get; set; } = new List<string>();
        public string[] Tags { get; set; }
    }

    public class Federation 
    {
        public FederationCluster Current { get; set; }
        public FederationCluster[] Clusters { get; set; }

        public FederationCluster GetCluster(string id)
        {
            if(Current.Id == id)
            {
                return Current;
            }
            else
            {
                foreach (var cluster in Clusters)
                {
                    if(cluster.Id == id)
                    {
                        return cluster;
                    }
                }
            }
            throw new ArgumentOutOfRangeException($"Cluster {id} not found in federation.");
        }

    }

}
