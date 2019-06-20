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
        public string[] Ids { get; }


        public PeerFilter()
        {
            Type = PeerFilterType.MatchSceneHost;
        }

        public PeerFilter(PeerFilterType type)
        {
            Type = type;
        }

        public PeerFilter(string id)
        {
            Type = PeerFilterType.MatchPeers;
            Ids = new string[] { id };
        }

        public PeerFilter(string[] ids)
        {
            Type = PeerFilterType.MatchPeers;
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
                        string[] peers = new string[nbPeers];
                        for (int i = 0; i < nbPeers; i++)
                        {
                            peers[i] = reader.ReadString();
                        }
                        return new PeerFilter(peers);
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

        public static PeerFilter MatchPeers(string id)
        {
            return new PeerFilter(id);
        }

        public static PeerFilter MatchPeers(string[] ids)
        {
            return new PeerFilter(ids);
        }

        public static PeerFilter MatchAllP2P()
        {
            return new PeerFilter(PeerFilterType.MatchAllP2P);
        }
    }
}
