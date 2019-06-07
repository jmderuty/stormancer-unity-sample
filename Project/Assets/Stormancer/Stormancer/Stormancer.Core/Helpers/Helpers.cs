using System.Collections.Generic;
using System.Threading;

namespace Stormancer
{
    class CancellationTokenHelpers
    {
        public static CancellationToken CreateLinkedShutdownToken(CancellationToken token)
        {
            return CreateLinkedSource(token, Shutdown.Instance.GetShutdownToken()).Token;
        }

        public static CancellationTokenSource CreateLinkedSource(CancellationToken token1, CancellationToken token2, CancellationToken token3)
        {
            List<CancellationToken> tokens = new List<CancellationToken>();
            if (token1.CanBeCanceled)
            {
                tokens.Add(token1);
            }
            if (token2.CanBeCanceled)
            {
                tokens.Add(token2);
            }
            if (token3.CanBeCanceled)
            {
                tokens.Add(token3);
            }
            return CancellationTokenSource.CreateLinkedTokenSource(tokens.ToArray());
        }

        public static CancellationTokenSource CreateLinkedSource(CancellationToken token1, CancellationToken token2)
        {
            List<CancellationToken> tokens = new List<CancellationToken>();
            if (token1.CanBeCanceled)
            {
                tokens.Add(token1);
            }
            if (token2.CanBeCanceled)
            {
                tokens.Add(token2);
            }
            return CancellationTokenSource.CreateLinkedTokenSource(token1, token2);
        }
    }
}
