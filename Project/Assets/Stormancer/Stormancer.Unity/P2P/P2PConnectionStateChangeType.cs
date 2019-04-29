
namespace Stormancer
{
    enum P2PConnectionStateChangeType
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

    enum EndpointCandidateType
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
