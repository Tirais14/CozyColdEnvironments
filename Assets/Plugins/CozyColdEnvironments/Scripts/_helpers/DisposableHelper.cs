using CCEnvs.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    public static class DisposableHelper
    {
        public static void DisposeEach<T>(this IEnumerable<T>? disposables)
            where T : IDisposable
        {
            if (disposables.IsNull())
                return;

            using var disposablesPooled = disposables.Cast<IDisposable>().EnumerableToArrayPooled();

            foreach (var item in disposablesPooled.GetSpan())
            {
                try
                {
                    item.Dispose();
                }
                catch (Exception ex)
                {
                    item.PrintException(ex);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeEachAndClear(this ICollection<IDisposable>? source)
        {
            if (source.IsNull())
                return;

            source.DisposeEach();
            source.Clear();
        }
    }
}
