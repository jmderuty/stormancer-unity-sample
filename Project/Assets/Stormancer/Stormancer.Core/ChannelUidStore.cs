
using System;
using System.Collections.Concurrent;

namespace Stormancer
{
    class ChannelUidStore
    {
        private class ChannelInfos
        {
            public string ChannelIdentifier;
            public DateTime LastUsed;
        };


        public ChannelUidStore()
        {
            var now = DateTime.UtcNow;

            for (int channelUid = 0; channelUid < ChannelUidsCount; channelUid++)
            {
                var channelInfo = new ChannelInfos();
                channelInfo.LastUsed = now;
                _channelsInfos.TryAdd(channelUid,channelInfo);
            }
        }

        public int GetChannelUid(string channelIdentifier)
        {
            ChannelInfos channelInfos;
            // Look for reserved channelUid by channelIdentifier

            int channelUid = GetReservedChannel(channelIdentifier);
            if(channelUid == -1)
            {
                // Get the older used channel
                channelUid = GetOlderChannel();

                // This has to return a correct value
                if(channelUid == -1)
                {
                    throw new Exception("No channelUid available.");
                }

                // Reserve the channelUid for this channelIdentifier
                channelInfos = _channelsInfos[channelUid];
                channelInfos.ChannelIdentifier = channelIdentifier;
            }

            // Update the channelUid last used date
            channelInfos = _channelsInfos[channelUid];
            channelInfos.LastUsed = DateTime.UtcNow;
            return channelUid;
        }

        private int GetReservedChannel(string channelIdentifier)
        {
            for (int channelUid = 0; channelUid < ChannelUidsCount; channelUid++)
            {
               var channelInfos = _channelsInfos[channelUid];
               if(channelInfos.ChannelIdentifier == channelIdentifier)
                {
                    return channelUid;
                }
            }
            return -1;
        }

		private int GetOlderChannel()
        {
            int minChannelUid = -1;
            var minDate = DateTime.UtcNow;

            for (int channelUid = 0; channelUid < ChannelUidsCount; channelUid++)
            {
                var channelInfos = _channelsInfos[channelUid];
                if (channelInfos.LastUsed < minDate)
                {
                    minDate = channelInfos.LastUsed;
                    minChannelUid = channelUid;
                }
            }

            return minChannelUid;
        }

        private static int ChannelUidsCount = 16;
        private ConcurrentDictionary<int, ChannelInfos> _channelsInfos = new ConcurrentDictionary<int, ChannelInfos>();

    };
}