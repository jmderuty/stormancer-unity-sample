﻿using Stormancer.Networking;
using System;
using System.Collections.Concurrent;

namespace Stormancer
{
    public class P2PSessions
    {
        private IConnectionManager _connections;
        private ConcurrentDictionary<P2PSessionId, P2PSession> _sessions = new ConcurrentDictionary<P2PSessionId, P2PSession>();

        public P2PSessions(IConnectionManager connections)
        {
            _connections = connections;
        }

        public void UpdateSessionState(P2PSessionId sessionId, P2PSessionState sessionState)
        {
            P2PSession session;
            if (_sessions.TryGetValue(sessionId, out session))
            {
                session.Status = sessionState;
            }
        }

        public void CreateSession(P2PSessionId sessionId, P2PSession session)
        {
            if(!_sessions.TryAdd(sessionId, session))
            {
                throw new InvalidOperationException("Session already created");
            }
        }

        public bool CloseSession(P2PSessionId sessionId)
        {
            P2PSession session;
            if (!_sessions.TryRemove(sessionId, out session))
            {
                throw new InvalidOperationException("Session not found");
            }
            var connection = _connections.GetConnection(session.RemotePeer);
            if(connection == null)
            {
                return false;
            }
            else
            {
                connection.Close();
                return true;
            }
        }
    }
}