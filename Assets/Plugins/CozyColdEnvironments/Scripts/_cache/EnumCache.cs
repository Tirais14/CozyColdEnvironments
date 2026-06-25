using CCEnvs.Caching;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace CCEnvs.Utils
{
    public static class EnumCache
    {
        private readonly static Cache<Type, Enum[]> cache = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        public static Enum[] GetFieldValues(Type type)
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsTrue(type.IsEnum, nameof(type), "Is not enum");

            if (!cache.TryGetValue(type, out var values))
            {
                values = Enum.GetValues(type).Cast<Enum>().ToArray();

                if (cache.TryAdd(type, values, out var entry))
                    entry.ExpirationTimeRelativeToNow = 10.Minutes();
            }

            return values;
        }
    }

    public static class EnumCache<T>
        where T : Enum
    {
        private static ImmutableArray<T>? values;

        public static ImmutableArray<T> Values {
            get
            {
                values ??= Enum.GetValues(typeof(T)).Cast<T>().ToImmutableArray();
                return values.Value;
            }
        }
    }
}