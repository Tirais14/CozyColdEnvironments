using CCEnvs.Collections;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs
{
    public static class UtilizableHelper
    {
        public static void TryUtilizeOrDispose(this object source)
        {
            if (source.IsNull())
                return;

            if (source.Is<IUtilizable>(out var utilizable))
                utilizable.Utilize();
            else if (source.Is<IDisposable>(out var disposable))
                disposable.Dispose();
        }

        public static void UtilizeEach<T>(this IEnumerable<T>? source, bool bufferized = true)
            where T : IUtilizable
        {
            if (source.IsNullOrEmpty())
                return;

            if (bufferized)
            {
                using var utilizables = source.Cast<IUtilizable>().EnumerableToArrayPooled();

                foreach (var item in utilizables.AsSpan())
                {
                    try
                    {
                        item.Utilize();
                    }
                    catch (Exception ex)
                    {
                        item.PrintException(ex);
                    }
                }
            }
            else
            {
                foreach (var item in source)
                {
                    try
                    {
                        item.Utilize();
                    }
                    catch (Exception ex)
                    {
                        item.PrintException(ex);
                    }
                }
            }
        }

        public static void UtilizeEachAndClear<T>(this ICollection<T>? source, bool bufferized = true)
            where T : IUtilizable
        {
            if (source.IsNullOrEmpty())
                return;

            source.UtilizeEach(bufferized);
            source.Clear();
        }

        public static void UtilizeOrDisposeEach<T>(this IEnumerable<T>? source, bool bufferized = true)
        {
            if (TypeofCache<T>.Type.IsType<IUtilizable>())
                source.Cast<IUtilizable>().UtilizeEach(bufferized);
            else if (TypeofCache<T>.Type.IsType<IDisposable>())
                source.Cast<IDisposable>().DisposeEach(bufferized);
        }

        public static void UtilizeOrDisposeAndClear<T>(this ICollection<T> source, bool bufferized = true)
        {
            source.UtilizeOrDisposeEach(bufferized);
            source.Clear();
        }
    }
}
