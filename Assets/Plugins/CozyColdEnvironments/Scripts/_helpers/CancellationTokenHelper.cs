using CCEnvs.Collections;
using System.Threading;

#nullable enable
namespace CCEnvs
{
    public static class CancellationTokenHelper
    {
        public static void CheckCancellationRequestByInterval(
            this in CancellationToken cancellationToken,
            ref int frame,
            int frameInterval = 5)
        {
            frame++;

            if (frame > frameInterval)
            {
                frame = 0;
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public static CancellationTokenSource LinkTokens(
            this CancellationToken source, 
            params CancellationToken[] cancellationTokens)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(source);
        }
    }
}
