using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Commands;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveSystem
    {
        public const int MAX_IO_OPERATIONS = 2;

        public static IReadOnlyDictionary<Type, SnapshotFactory> Converters => converters;

        public static SnapshotFactory DefaultConverter { get; } =
            static (obj) =>
            {
                return new ValueSnapshot<object>(obj);
            };

        public static CommandScheduler CommandScheduler { get; } = new(UnityFrameProvider.Update, nameof(SaveSystem));

        internal static SemaphoreSlim IOSemaphore {
            get
            {
                _ioSemaphore ??= new SemaphoreSlim(
                    MAX_IO_OPERATIONS,
                    MAX_IO_OPERATIONS
                    );

                return _ioSemaphore;
            }
        }

        private static SemaphoreSlim? _ioSemaphore;

        private readonly static Dictionary<Type, SnapshotFactory> converters = new();

        public static void RegisterType(Type type, SnapshotFactory converter)
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsNotNull(converter, nameof(converter));

            converters.Add(type, converter);
        }
        public static void RegisterType<T>(SnapshotFactory<T> converter)
        {
            Guard.IsNotNull(converter, nameof(converter));

            RegisterType(typeof(T), (obj) => converter((T)obj));
        }

        public static bool UnregisterType(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            return converters.Remove(type);
        }

        public static bool UnregisterType<T>()
        {
            return UnregisterType(typeof(T));
        }

        public static SnapshotFactory ResolveConverter(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            if (!Converters.TryGetValue(type, out var converter))
            {
                if (CCDebug.Instance.IsEnabled)
                    typeof(SaveSystem).PrintWarning($"Cannot resolve the converter for: {type}. The default converter is used");

                return DefaultConverter;
            }

            return converter;
        }

        public static SnapshotFactory ResolveConverter<T>()
        {
            return ResolveConverter(typeof(T));
        }
    }
}
