using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Stormancer.Plugins
{
    public class PartyInvitations
    {
        private Dictionary<string, PartyInvitation> _partyInvitations;
        private Dictionary<string, CancellationTokenSource> _partyRequests;

        public PartyInvitation ReceivePartyInvitation(string senderId, string sceneId)
        {
            var invitation = new PartyInvitation(senderId, sceneId);
            
            if(!_partyInvitations.ContainsKey(senderId))
            {
                _partyInvitations.Add(senderId, invitation);
            }
            else
            {
                throw new InvalidOperationException("Could not receive party invitation as there is already one from " + senderId);
            }
            return invitation;
        }

        public void RemovePartyInvitation(string senderId)
        {
            if(_partyInvitations.ContainsKey(senderId))
            {
                _partyInvitations.Remove(senderId);
            }
            else
            {
                throw new InvalidOperationException("Could not remove pending party invitation as there is none from " + senderId);
            }
        }

        public void AnswerPartyInvitation(string senderId, bool accept)
        {
            if (_partyInvitations.ContainsKey(senderId))
            {
                _partyInvitations[senderId].OnAnswer?.Invoke(accept);
                RemovePartyInvitation(senderId);
            }
            else
            {
                throw new InvalidOperationException("Could not answer pending party invitation as there is none from " + senderId);
            }
        }

        public List<PartyInvitation> GetPartyInvitations()
        {
            return _partyInvitations.Values.ToList();
        }

        public void SendPartyRequest(string userId, CancellationTokenSource cts)
        {
            if(!_partyRequests.ContainsKey(userId))
            {
                _partyRequests.Add(userId, cts);
            }
            else
            {
                throw new InvalidOperationException("Could not send a party request as there is already one for " + userId);
            }
        }

        public void CancelPartyRequest(string userId)
        {
            CancellationTokenSource request;
            if(_partyRequests.TryGetValue(userId, out request))
            {
                request.Cancel();
                _partyRequests.Remove(userId);
            }
            else
            {
                throw new InvalidOperationException("Could not cancel a party request as there none pending for " + userId);
            }
        }

        public List<string> GetPartyRequestUserIds()
        {
            return _partyRequests.Keys.ToList();
        }
    }
}