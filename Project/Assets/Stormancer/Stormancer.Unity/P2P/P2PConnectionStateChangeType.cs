
namespace Stormancer
{
    public enum P2PConnectionStateChangeType
    {
        Connecting,
		Connected,
		Disconnected,
	};

    public enum P2PSessionState
    {

        Connecting,
		Connected,
		Closing,
		Unknown,
	};

    public enum EndpointCandidateType
    {
        Nat,
		Private,
		Public
    };

    public enum P2PTunnelSide
    {
        Host,
		Client
    };
}
