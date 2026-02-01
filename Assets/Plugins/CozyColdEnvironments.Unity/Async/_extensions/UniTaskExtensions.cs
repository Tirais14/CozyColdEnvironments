using CCEnvs.Diagnostics;
using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Unity.Async
{
    public static class UniTaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForgetByPrintException(this UniTask source, object? context = null)
        {
            if (context.IsNull())
                source.Forget(static ex => CCDebug.Instance.PrintException(ex));
            else
                source.Forget(ex => context.PrintException(ex));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForgetByPrintException<T>(this UniTask<T> source, object? context = null)
        {
            if (context.IsNull())
                source.Forget(static ex => CCDebug.Instance.PrintException(ex));
            else
                source.Forget(ex => context.PrintException(ex));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForgetByThrow(this UniTask source, object? context = null)
        {
            source.Forget(static ex => throw ex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForgetByThrow<T>(this UniTask<T> source, object? context = null)
        {
            source.Forget(static ex => throw ex);
        }
    }
}
