﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer.Core
{
    /// <summary>
    /// Represents a route on a scene.
    /// </summary>
    public class Route
    {
        public Route()
        {
            Handle = 0;
        }

        public Route(string routeName, ushort handle, MessageOriginFilter filter, Dictionary<string, string> metadata)
        {
            Name = routeName;
            Filter = filter;
            if (metadata == null)
            {
                metadata = new Dictionary<string, string>();
            }
            Metadata = metadata;
            Handle = handle;
        }

        public Route(string routeName, Dictionary<string, string> metadata)
        {
            Name = routeName;
            Filter = MessageOriginFilter.Host;
            if (metadata == null)
            {
                metadata = new Dictionary<string, string>();
            }
            Metadata = metadata;
            Handle = 0;
        }

        /// <summary>
        /// A string containing the name of the route.
        /// </summary>
        public string Name { get; }
        public MessageOriginFilter Filter { get;}
        public ushort Handle { get; set; }
        public Dictionary<string, string> Metadata { get; }

        public Action<Packet> Handlers { get; set; }
    }
}