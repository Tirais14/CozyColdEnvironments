using CCEnvs.Collections;
using System;
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
            return CancellationTokenSource.CreateLinkedTokenSource(cancellationTokens.PrependToArray(source));
        }

        public static CancellationTokenSource LinkTokens(
            this CancellationToken source,
            CancellationToken other)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(source, other);
        }

        public static CancellationTokenSource LinkTokens(
            this CancellationToken source,
            out CancellationToken result,
            params CancellationToken[] cancellationTokens)
        {
            var tokenSource = source.LinkTokens(cancellationTokens);

            result = tokenSource.Token;

            return tokenSource;
        }

        public static CancellationTokenSource LinkTokens(
            this CancellationToken source,
            CancellationToken other,
            out CancellationToken result)
        {
            var tokenSource = source.LinkTokens(other);

            result = tokenSource.Token;

            return tokenSource;
        }

        public static CancellationTokenSource? TryLinkTokens(
            this CancellationToken source,
            params CancellationToken[] cancellationTokens)
        {
            bool hasValidToken = false;

            for (int i = 0; i < cancellationTokens.Length; i++)
            {
                if (cancellationTokens[i].CanBeCanceled)
                {
                    hasValidToken = true;
                    break;
                }
            }

            if (!hasValidToken)
                return null;

            return source.LinkTokens(cancellationTokens);
        }

        public static CancellationTokenSource? TryLinkTokens(
            this CancellationToken source,
            CancellationToken other)
        {
            if (!other.CanBeCanceled)
                return null;

            return source.LinkTokens(other);
        }

        public static CancellationTokenSource? TryLinkTokens(
            this CancellationToken source,
            out CancellationToken result,
            params CancellationToken[] cancellationTokens)
        {
            if (!source.TryLinkTokens(cancellationTokens).Let(out var tokenSource))
            {
                result = source;

                return null;
            }

            result = tokenSource.Token;

            return tokenSource;
        }

        public static CancellationTokenSource? TryLinkTokens(
            this CancellationToken source,
            CancellationToken other, 
            out CancellationToken result)
        {
            if (!source.TryLinkTokens(other).Let(out var tokenSource))
            {
                result = source;

                return null;
            }

            result = tokenSource.Token;

            return tokenSource;
        }
    }
}
