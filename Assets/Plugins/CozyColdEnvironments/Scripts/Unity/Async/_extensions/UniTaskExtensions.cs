using CCEnvs.Diagnostics;
using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Unity.Async
{
    public static class UniTaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(UniTask source, object? context = null)
        {
            if (context.IsNull())
                source.Forget(ex => CCDebug.PrintException(ex));
            else
                source.Forget(ex => context.PrintException(ex));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget<T>(UniTask<T> source, object? context = null)
        {
            if (context.IsNull())
                source.Forget(ex => CCDebug.PrintException(ex));
            else
                source.Forget(ex => context.PrintException(ex));
        }
    }
}
