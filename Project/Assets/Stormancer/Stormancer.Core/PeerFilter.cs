using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace Stormancer
{
    enum PeerFilterType
    {
        MatchAllFilter,
		MatchSceneHost,
		MatchPeerFilter,
		MatchArrayFilter
    };

    public class PeerFilter
    {
        PeerFilter(PeerFilterType type)
        {
            Type = type;
        }

        PeerFilter(PeerFilterType type, long id)
        {
            Type = type;
            Ids = new long[] { id };
        }

        PeerFilter(PeerFilterType type, long[] ids)
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
                    case 0:
                        {
                            long id = reader.ReadInt64();
                            return new PeerFilter(PeerFilterType.MatchPeerFilter, id);
                        }
                    case 1:
                        {
                            ushort nbPeers = reader.ReadUInt16();
                            long[] peers = new long[nbPeers];
                            for (int i = 0; i < nbPeers; i++)
                            {
                                peers[i] = reader.ReadInt64();
                            }
                            return new PeerFilter(PeerFilterType.MatchArrayFilter, peers);
                        }
                    case 2:
                        {
                            return new PeerFilter(PeerFilterType.MatchAllFilter);
                        }
                    default:
                        {
                            return new PeerFilter(PeerFilterType.MatchSceneHost);
                        }
                }
            }
        }

        PeerFilterType Type = PeerFilterType.MatchSceneHost;
        long[] Ids;

        public static PeerFilter MatchAllFilter()
        {
            return new PeerFilter(PeerFilterType.MatchAllFilter);
        }

        public static PeerFilter MatchSceneHost()
        {
            return new PeerFilter(PeerFilterType.MatchSceneHost);
        }

        public static PeerFilter MatchPeerFilter(long id)
        {
            return new PeerFilter(PeerFilterType.MatchPeerFilter, id);
        }

        public static PeerFilter MatchArrayFilter(long[] ids)
        {
            return new PeerFilter(PeerFilterType.MatchArrayFilter, ids);
        }
    }
}
