﻿// :!Serialize
using MsgPack.Serialization;

namespace Stormancer.Plugins
{
    [MsgPackDto]
    public class Player
    {
        [MessagePackMember(0)]
        public string PlayerId { get; set; }

        [MessagePackMember(1)]
        public string Pseudo { get; set; }
    }
}