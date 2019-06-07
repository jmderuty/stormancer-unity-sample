using MsgPack.Serialization;
using System;

namespace Stormancer.Plugins
{
    [MsgPackDto]
    public class PartyRequestDto
    {
        [MessagePackMember(0)]
        public string PlatformSessionId { get; set; }
        [MessagePackMember(1)]
        public ulong PartySize { get; set; }
        [MessagePackMember(2)]
        public string GameFinderName { get; set; }
        [MessagePackMember(3)]
        public string CustomData { get; set; }
        [MessagePackMember(4)]
        public bool StartOnlyIfPartyFull { get; set; }
    }

    public class PartyInvitation
    {
        public string UserId { get; private set; }
        public string SceneId { get; private set; }
        public Action<bool> OnAnswer;

        public PartyInvitation(string userId, string sceneId)
        {
            UserId = userId;
            SceneId = sceneId;
        }
    }

    public enum PartyUserStatus
    {
        NotReady = 0,
        Ready = 1
    }

    [MsgPackDto]
    public class PartyUserDto
    {
        [MessagePackMember(0)]
        public string UserId { get; set; }
        [MessagePackMember(1)]
        public bool IsLeader { get; set; }
        [MessagePackMember(2)]
        public PartyUserStatus UserStatus { get; set; }
        [MessagePackMember(3)]
        public string UserData { get; set; }
    }

    [MsgPackDto]
    public class PartyUserData
    {
        [MessagePackMember(0)]
        public string UserId { get; set; }
        [MessagePackMember(1)]
        public string UserData { get; set; }
    }

    [MsgPackDto]
    public class PartySettingsDto
    {
        [MessagePackMember(0)]
        public string GameFinderName { get; set; }
        [MessagePackMember(1)]
        public string LeaderId { get; set; }
        [MessagePackMember(2)]
        public ushort PartySize { get; set; }
        [MessagePackMember(3)]
        public string CustomData { get; set; }
        [MessagePackMember(4)]
        public bool StartOnlyIfPartyFull { get; set; }
    }

    public class PartySettings
    {
        public string GameFinderName { get; set; }
        public string LeaderId { get; set; }
        public ushort PartySize { get; set; }
        public string CustomData { get; set; }
        public bool StartOnlyIfPartyFull { get; set; }
    }


}