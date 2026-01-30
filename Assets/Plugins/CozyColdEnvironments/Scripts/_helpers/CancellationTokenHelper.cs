using CCEnvs.Collections;
using System.Threading;

#nullable enable
namespace CCEnvs
{
    public static class CancellationTokenHelper
    {
        public static void ThrowIfCancellationRequestedByIntervalAndMoveNext(
            this in CancellationToken cancellationToken,
            ref int frame,
            int frameInterval = 3)
        {
            if (frameInterval == 0 || (frame % frameInterval) == 0)
            {
                frame = 0;
                cancellationToken.ThrowIfCancellationRequested();
            }

            frame++;
        }

        public static void ThrowIfCancellationRequestedByIntervalAndMoveNext(
            this in CancellationToken cancellationToken,
            ref long frame,
            long frameInterval = 10L)
        {
            if (frameInterval == 0 || (frame % frameInterval) == 0)
            {
                frame = 0L;
                cancellationToken.ThrowIfCancellationRequested();
            }

            frame++;
        }

        public static void ThrowIfCancellationRequestedByInterval(
            this in CancellationToken cancellationToken,
            int frame,
            int frameInterval = 3)
        {
            if (frameInterval == 0 || (frame % frameInterval) == 0)
                cancellationToken.ThrowIfCancellationRequested();
        }

        public static void ThrowIfCancellationRequestedByInterval(
            this in CancellationToken cancellationToken,
            long frame,
            long frameInterval = 10L)
        {
            if (frameInterval == 0L || (frame % frameInterval) == 0)
                cancellationToken.ThrowIfCancellationRequested();
        }

        public static CancellationTokenSource LinkTokens(
            this CancellationToken source, 
            params CancellationToken[] cancellationTokens)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(source);
        }
    }
}
