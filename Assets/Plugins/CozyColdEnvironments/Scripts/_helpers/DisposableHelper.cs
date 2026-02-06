using CCEnvs.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CCEnvs
{
    public static class DisposableHelper
    {
        public static void DisposeEach<T>(this IEnumerable<T>? disposables)
            where T : IDisposable
        {
            if (disposables.IsNull())
                return;

            if (disposables.IsMutableCollection())
            {
                using var disposablesPooled = disposables.Cast<IDisposable>().EnumerableToArrayPooled();

                foreach (var item in disposablesPooled.Value)
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

            foreach (var item in disposables)
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
