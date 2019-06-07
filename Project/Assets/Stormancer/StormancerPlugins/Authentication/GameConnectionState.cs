
namespace Stormancer.Plugins
{
    public enum GameConnectionState
    {
        Disconnected,
        Connecting,
        Authenticated,
        Disconnecting,
        Authenticating,
        Reconnecting
    }

    public class GameConnectionStateCtx
    {
        public GameConnectionStateCtx(GameConnectionState state, string reason = null)
        {
            State = state;
            if(!string.IsNullOrEmpty(reason))
            {
                Reason = reason;
            }
        }
        
        public GameConnectionState State { get; set; }
        public string Reason { get; set; }
    }
}
