using System.Threading;

#nullable enable
namespace CCEnvs.Threading
{
    public static class CancellationTokenSourceHelper
    {
        public static void CancelAndDispose(this CancellationTokenSource source)
        {
            CC.Guard.IsNotNullSource(source);

            source.Cancel();
            source.Dispose();
        }

        public static void CancelAndDispose(ref CancellationTokenSource source)
        {
            CC.Guard.IsNotNullSource(source);

            source.Cancel();
            source.Dispose();

            source = null!;
        }
    }
}
