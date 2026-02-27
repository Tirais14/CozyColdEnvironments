using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Commands;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public static class SaveSystem 
    {
        public static IReadOnlyDictionary<Type, Func<object, ISnapshot>> Converters => converters;

        public static Func<object, ISnapshot> DefaultConverter { get; } = (obj) => new ValueSnapshot(obj);

        public static CommandScheduler CommandScheduler { get; } = new(UnityFrameProvider.Update, nameof(SaveSystem));

        internal static SemaphoreSlim readWriteSemaphore {
            get
            {
                _readWriteSemaphore ??= new SemaphoreSlim(
                    Environment.ProcessorCount * 2,
                    Environment.ProcessorCount * 2
                    );

                return _readWriteSemaphore;
            }
        }

        private static SemaphoreSlim? _readWriteSemaphore;

        private readonly static Dictionary<Type, Func<object, ISnapshot>> converters = new();

        public static void RegisterType(Type type, Func<object, ISnapshot> converter)
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsNotNull(converter, nameof(converter));

            converters.Add(type, converter);
        }
        public static void RegisterType<T>(Func<object, ISnapshot> converter)
        {
            RegisterType(typeof(T), converter);
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

        public static Func<object, ISnapshot> ResolveConverter(Type type)
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

        public static Func<object, ISnapshot> ResolveConverter<T>()
        {
            return ResolveConverter(typeof(T));
        }
    }
}
