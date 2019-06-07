﻿// :!Serialize
using MsgPack.Serialization;
using System;
using System.Collections.Generic;

namespace Stormancer.Cluster.Application
{
    [MsgPackDto]
    public class ConnectionData
    {
        public Dictionary<string, string> Endpoints { get; set; }

        public string AccountId { get; set; }
        public string Application { get; set; }

        public string SceneId { get; set; }

        public string Routing { get; set; }

        public DateTime Issued { get; set; }

        public DateTime Expiration { get; set; }

        public byte[] UserData { get; set; }

        public string ContentType { get; set; }

        public int Version { get; set; }

        public string DeploymentId { get; set; }
    }
}
