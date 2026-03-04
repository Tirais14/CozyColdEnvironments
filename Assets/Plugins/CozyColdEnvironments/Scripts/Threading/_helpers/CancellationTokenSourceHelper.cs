using System.Threading;

#nullable enable
namespace CCEnvs.Threading
{
    public static class CancellationTokenSourceHelper
    {
        public static void CancelAndDispose(this CancellationTokenSource source)
        {
            CC.Guard.IsNotNullSource(source);

            try
            {
                source.Cancel();
            }
            catch (System.Exception ex)
            {
                typeof(CancellationTokenSourceHelper).PrintException(ex);
            }
            finally
            {
                source.Dispose();
            }
        }

        public static void CancelAndDispose(ref CancellationTokenSource source)
        {
            CC.Guard.IsNotNullSource(source);

            try
            {
                source.Cancel();
            }
            catch (System.Exception)
            {
                typeof(CancellationTokenSourceHelper).PrintException(ex);
            }
            finally
            {
                source.Dispose();
                source = null!;
            }
        }
    }
}
