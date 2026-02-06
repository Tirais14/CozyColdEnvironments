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
        public static void UtilizeOrDispose(this object source)
        {
            if (source.IsNull())
                return;

            if (source.Is<IUtilizable>(out var utilizable))
            {
                utilizable.Utilize();
                return;
            }
            else if (source.Is<IDisposable>(out var disposable))
                disposable.Dispose();
        }

        public static void UtilizeEach<T>(this IEnumerable<T>? source)
            where T : IUtilizable
        {
            if (source.IsNullOrEmpty())
                return;

            if (source.IsMutableCollection())
            {
                foreach (var item in source.Cast<IUtilizable>().EnumerableToArrayPooled())
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

        public static void UtilizeEachAndClear<T>(this ICollection<T>? source)
            where T : IUtilizable
        {
            if (source.IsNullOrEmpty())
                return;

            source.UtilizeEach();

            if (source.IsMutableCollection())
                source.Clear();
        }

        public static void UtilizeOrDisposeEach<T>(this IEnumerable<T>? source)
        {
            if (CachedTypeof<T>.Type.IsType<IUtilizable>())
                source.Cast<IUtilizable>().UtilizeEach();
            else if (CachedTypeof<T>.Type.IsType<IDisposable>())
                source.Cast<IDisposable>().DisposeEach();
        }

        public static void UtilizeOrDisposeAndClear<T>(this ICollection<T> source)
        {
            source.UtilizeOrDisposeEach();

            if (source.IsMutableCollection())
                source.Clear();
        }
    }
}
