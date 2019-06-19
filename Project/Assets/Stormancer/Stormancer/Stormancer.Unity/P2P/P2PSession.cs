using MsgPack.Serialization;

namespace Stormancer
{
    [MsgPackDto]
    public class P2PSession
    {
        [MessagePackMember(0)]
        public byte[] SessionId { get; set; }

        [MessagePackMember(1)]
        public ulong RemotePeer { get; set; }

        [MessagePackMember(2)]
        public string SceneId { get; set; }

        public P2PSessionState Status { get; set; }


        public P2PSession()
        {
            RemotePeer = 0;
            Status = P2PSessionState.Unknown;
        }

    }
}