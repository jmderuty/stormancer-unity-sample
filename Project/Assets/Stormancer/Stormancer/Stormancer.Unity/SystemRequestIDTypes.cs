﻿
namespace Stormancer.Client45
{
    enum SystemRequestIDTypes : byte
    {

        ID_SET_METADATA = 0,
        ID_SCENE_READY = 1,
        ID_PING = 2,

        // P2P
        ID_P2P_CREATE_SESSION = 3,
        ID_P2P_CLOSE_SESSION = 4,
        ID_P2P_GATHER_IP = 5,
        ID_P2P_TEST_CONNECTIVITY_HOST = 6,
        ID_P2P_TEST_CONNECTIVITY_CLIENT = 7,
        ID_P2P_CONNECT_HOST = 8,
        ID_P2P_CONNECT_CLIENT = 9,
        ID_P2P_CLOSE_TUNNEL = 10,
        ID_P2P_OPEN_TUNNEL = 11,

        ID_P2P_RELAY_OPEN = 12,
        ID_P2P_RELAY_CLOSE = 13,
        // ...

        ID_JOIN_SESSION = 14,
        ID_CONNECT_TO_SCENE = 134,
        ID_DISCONNECT_FROM_SCENE = 135,
        ID_GET_SCENE_INFOS = 136
       
    }
}
