using CommunityToolkit.Diagnostics;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;

#nullable enable
namespace CCEnvs.Utils
{
    public static class EnumCache 
    {
        private readonly static MemoryCache cache = new(new MemoryCacheOptions
        {
            ExpirationScanFrequency = 1.Minutes()
        });

        public static Enum[] GetFieldValues(Type type)
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsTrue(type.IsEnum, nameof(type), "Is not enum");

            if (!cache.TryGetValue(type, out object? values))
            {
                var entry = cache.CreateEntry(type);

                values = Enum.GetValues(type).Cast<Enum>().ToArray();

                entry.Value = values;
                entry.AbsoluteExpirationRelativeToNow = 10.Minutes();
            }

            return (Enum[])values!;
        }
    }

    public static class EnumCache<T>
        where T : Enum
    {
        private static T[]? value;

        public static T[] Values {
            get
            {
                value ??= Enum.GetValues(typeof(T)).Cast<T>().ToArray();

                return value;
            }
        }
    }
}