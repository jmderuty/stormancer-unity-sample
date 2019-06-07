// :!Serialize
using MsgPack.Serialization;

namespace Stormancer.Core.Infrastructure.Messages
{
    [MsgPackDto]
    public class SystemResponse
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}
