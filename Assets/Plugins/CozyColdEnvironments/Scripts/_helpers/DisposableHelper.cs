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
        public static void DisposeEach<T>(this IEnumerable<T>? disposables, bool bufferized = true)
            where T : IDisposable
        {
            if (disposables.IsNull())
                return;

            if (bufferized)
            {
                using var disposablesPooled = disposables.Cast<IDisposable>().EnumerableToArrayPooled();

                foreach (var item in disposablesPooled.AsSpan())
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
            else
            {
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeEachAndClear(this ICollection<IDisposable>? source, bool bufferized = true)
        {
            if (source.IsNull())
                return;

            source.DisposeEach(bufferized);
            source.Clear();
        }
    }
}
