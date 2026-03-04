using System;
using System.Collections.Generic;
using System.Threading;
using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Commands;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using ObservableCollections;
using R3;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveSystem
    {
        public const int MAX_IO_OPERATIONS = 2;

        private readonly static Dictionary<Type, SnapshotFactory> converters = new();

        [OnInstallResetable]
        private readonly static ObservableDictionary<string, SaveArchive> archives = new(1, null);

        private static SemaphoreSlim? _ioSemaphore;

        private static JsonSerializerSettings serializerSettings = GetDefaultSerializerSettings();

        public static IReadOnlyDictionary<Type, SnapshotFactory> Converters => converters;

        public static IReadOnlyObservableDictionary<string, SaveArchive> Archives => archives;

        public static SnapshotFactory DefaultConverter { get; } =
            static (obj) =>
            {
                return new ValueSnapshot<object>(obj);
            };

        public static JsonSerializerSettings SerializerSettings {
            get => serializerSettings;
            set => serializerSettings = value ?? GetDefaultSerializerSettings();
        }

        internal static CommandScheduler CommandScheduler { get; } = new(UnityFrameProvider.Update, nameof(SaveSystem));

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

        public static SaveArchive GetOrCreateArchive(string path)
        {
            Guard.IsNotNullOrWhiteSpace(path, nameof(path));

            if (!archives.TryGetValue(path, out var archive))
            {
                archive = new SaveArchive(path);

                archives.Add(path, archive);
            }

            return archive;
        }

        public static bool RemoveArchive(string path, out SaveArchive removed)
        {
            Guard.IsNotNullOrWhiteSpace(path, nameof(path));

            return archives.Remove(path, out removed);
        }

        public static bool RemoveArchive(string path)
        {
            Guard.IsNotNullOrWhiteSpace(path, nameof(path));

            return archives.Remove(path);
        }

        public static void ClearArchives()
        {
            archives.Clear();
        }

        public static void RegisterType(Type type, SnapshotFactory converter)
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsNotNull(converter, nameof(converter));

            converters[type] = converter;
        }
        public static void RegisterType<T>(SnapshotFactory<T> converter)
        {
            Guard.IsNotNull(converter, nameof(converter));

            RegisterType(typeof(T), (obj) => converter((T)obj));
        }

        public static void RegisterType<T>(T _, SnapshotFactory<T> converter)
        {
            Guard.IsNotNull(converter, nameof(converter));

            RegisterType(converter);
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

        private static JsonSerializerSettings GetDefaultSerializerSettings()
        {
            var serializerSettings = CC.SerializerSettings;
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            return serializerSettings;
        }

        //private static void InstallByAttributes(MemberInfo[] domainMembers)
        //{
        //    (from type in domainMembers.AsParallel().OfType<Type>()
        //     select (type, regTypeAttribute: type.GetCustomAttribute<RegisterSaveSystemTypeAttribute>()) into typeInfo
        //     where typeInfo.regTypeAttribute is not null
        //     select typeInfo)
        //    .ForAll(typeInfo =>
        //    {
        //        var ctors = typeInfo.type.GetConstructors(BindingFlagsDefault.InstancePublic)
        //            .Select(ctor => (ctor, prms: ctor.GetParameters()))
        //            .ToArray();

        //        if (ctors.IsEmpty())
        //        {
        //            typeof(SaveSystem).PrintError($"Cannot find any snapshot constructor");
        //            return;
        //        }

        //        if (ctors.FirstOrDefault(ctorInfo => ctorInfo.prms.Length > 0 && ctorInfo.prms[0].ParameterType == typeInfo.regTypeAttribute.SnapshotType)
        //            .IsNull(out var ctorInfo)
        //            &&
        //            CCDebug.Instance.IsEnabled)
        //        {
        //            typeof(SaveSystem).PrintLog($"Not found snapshot constructor of type: {typeInfo.regTypeAttribute.SnapshotType} and serach continued");
        //        }

        //        if (ctors.FirstOrDefault(ctorInfo => ctorInfo.prms.Length > 0 && ctorInfo.prms[0].ParameterType.IsType(typeInfo.regTypeAttribute.SnapshotType))
        //            .IsNull(out ctorInfo))
        //        {
        //            typeof(SaveSystem).PrintError($"Not found snapshot constructor of type: {typeInfo.regTypeAttribute.SnapshotType}");
        //            return;
        //        }

        //        var ctor = ctorInfo.ctor;

        //        SaveSystem.RegisterType(typeInfo.type, (obj) =>
        //        {
        //            ctor.Invoke(new object[] { obj });
        //        });
        //    });
        //}
    }
}
