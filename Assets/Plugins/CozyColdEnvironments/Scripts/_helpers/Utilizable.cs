using System;
using System.Collections.Generic;
using System.Linq;
using CCEnvs.Collections;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;

#nullable enable
namespace CCEnvs
{
    public static class Utilizable
    {
        public static void TryUtilizeOrDispose(object source)
        {
            if (source.IsNull())
                return;

            if (source.Is<IUtilizable>(out var utilizable))
                utilizable.Utilize();
            else if (source.Is<IDisposable>(out var disposable))
                disposable.Dispose();
        }

        public static void UtilizeEach<T>(IEnumerable<T>? source, bool bufferized = true)
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

        public static void UtilizeEachAndClear<T>(ICollection<T>? source, bool bufferized = true)
            where T : IUtilizable
        {
            if (source.IsNullOrEmpty())
                return;

            UtilizeEach(source ,bufferized);
            source.Clear();
        }

        public static void UtilizeOrDisposeEach<T>(IEnumerable<T>? source, bool bufferized = true)
        {
            if (TypeofCache<T>.Type.IsType<IUtilizable>())
                UtilizeEach(source.Cast<IUtilizable>(), bufferized);
            else if (TypeofCache<T>.Type.IsType<IDisposable>())
                source.Cast<IDisposable>().DisposeEach(bufferized);
        }

        public static void UtilizeOrDisposeAndClear<T>(ICollection<T> source, bool bufferized = true)
        {
            UtilizeOrDisposeEach(source, bufferized);
            source.Clear();
        }
    }
}
