using CCEnvs.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Unity.Async
{
    public static class UniTaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForgetByPrintException(this UniTask source, object? context = null)
        {
            if (context.IsNull())
            {
                source.Forget(
                    static ex =>
                    {
                        if (ex is TaskCanceledException || ex is OperationCanceledException)
                            CCDebug.Instance.PrintLog(ex);
                        else
                            CCDebug.Instance.PrintException(ex);

                    });
            }
            else
            {
                source.Forget(
                    ex =>
                    {
                        if (ex is TaskCanceledException || ex is OperationCanceledException)
                            context.PrintLog(ex);
                        else
                            context.PrintException(ex);

                    });
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForgetByPrintException<T>(this UniTask<T> source, object? context = null)
        {
            if (context.IsNull())
            {
                source.Forget(
                    static ex =>
                    {
                        if (ex is TaskCanceledException || ex is OperationCanceledException)
                            CCDebug.Instance.PrintLog(ex);
                        else
                            CCDebug.Instance.PrintException(ex);

                    });
            }
            else
            {
                source.Forget(
                    ex =>
                    {
                        if (ex is TaskCanceledException || ex is OperationCanceledException)
                            context.PrintLog(ex);
                        else
                            context.PrintException(ex);

                    });
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForgetByThrow(this UniTask source, object? context = null)
        {
            if (context.IsNull())
            {
                source.Forget(
                    static ex =>
                    {
                        if (ex is TaskCanceledException || ex is OperationCanceledException)
                            CCDebug.Instance.PrintLog(ex);
                        else
                            throw ex;

                    });
            }
            else
            {
                source.Forget(
                    ex =>
                    {
                        if (ex is TaskCanceledException || ex is OperationCanceledException)
                            context.PrintLog(ex);
                        else
                            throw ex;

                    });
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForgetByThrow<T>(this UniTask<T> source, object? context = null)
        {
            source.Forget(static ex => throw ex);
        }
    }
}
