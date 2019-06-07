using MsgPack.Serialization;

namespace Stormancer
{
    [MsgPackDto]
    public class EndpointCandidate
    {
        [MessagePackMember(0)]
        public string Address { get; set; }

        [MessagePackMember(1)]
        public EndpointCandidateType Type { get; set; }
    }
}