using System;
using System.Linq;
using CCEnvs.Caching;
using CommunityToolkit.Diagnostics;
using Humanizer;

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

            if (!cache.TryGet(type, out var values))
            {
                values = Enum.GetValues(type).Cast<Enum>().ToArray();

                if (cache.TryAdd(type, values, out var entry))
                    entry.ExpirationTimeRelativeToNow = 10.Minutes();
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