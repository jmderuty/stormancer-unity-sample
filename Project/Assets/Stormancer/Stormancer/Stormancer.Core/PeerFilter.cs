using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace Stormancer
{
    public enum PeerFilterType
    {
        MatchSceneHost = 0,
        MatchPeers = 1,
        MatchAllP2P = 2
    };

    public class PeerFilter
    {

        public PeerFilterType Type { get; } = PeerFilterType.MatchSceneHost;
        public ulong[] Ids { get; }


        public PeerFilter()
        {
            Type = PeerFilterType.MatchSceneHost;
        }

        public PeerFilter(PeerFilterType type)
        {
            Type = type;
        }

        public PeerFilter(PeerFilterType type, ulong id)
        {
            Type = type;
            Ids = new ulong[] { id };
        }

        public PeerFilter(PeerFilterType type, ulong[] ids)
        {
            Type = type;
            Ids = ids;
        }

        static PeerFilter ReadFilter(Stream stream)
        {
            byte peerType;
            peerType = (byte)stream.ReadByte();
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                switch (peerType)
                {
                    case 1:
                    {
                        ushort nbPeers = reader.ReadUInt16();
                        ulong[] peers = new ulong[nbPeers];
                        for (int i = 0; i < nbPeers; i++)
                        {
                            peers[i] = reader.ReadUInt64();
                        }
                        return new PeerFilter(PeerFilterType.MatchPeers, peers);
                    }
                    case 2:
                    {
                        return new PeerFilter(PeerFilterType.MatchAllP2P);
                    }
                    default:
                    {
                        return new PeerFilter(PeerFilterType.MatchSceneHost);
                    }
                }
            }
        }

        public static PeerFilter MatchSceneHost()
        {
            return new PeerFilter();
        }

        public static PeerFilter MatchPeers(ulong id)
        {
            return new PeerFilter(PeerFilterType.MatchPeers, id);
        }

        public static PeerFilter MatchPeers(ulong[] ids)
        {
            return new PeerFilter(PeerFilterType.MatchPeers, ids);
        }

        public static PeerFilter MatchAllP2P()
        {
            return new PeerFilter(PeerFilterType.MatchAllP2P);
        }
    }
}
