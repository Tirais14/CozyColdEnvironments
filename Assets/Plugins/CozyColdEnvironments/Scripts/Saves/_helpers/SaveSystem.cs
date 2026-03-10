using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveSystem
    {
        public const int MAX_IO_OPERATIONS =
#if PLATFORM_WEBGL
            1;
#else
            2;
#endif

        private readonly static Dictionary<Type, SnapshotFactory> converters = new();

        private readonly static object convertersSyncRoot = new();

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

        public static SaveObjectRestorer ObjectRestorer { get; } = new();

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

        static SaveSystem()
        {
            BindArchiveAdd();
            BindArchiveRemove();
            BindAcrhivesClear();
        }

        public static SaveArchive GetOrCreateArchive(string path)
        {
            Guard.IsNotNullOrWhiteSpace(path, nameof(path));

            SaveArchive? archive;

            lock (archives.SyncRoot)
            {
                if (!archives.TryGetValue(path, out archive))
                {
                    archive = new SaveArchive(path);

                    archives.Add(path, archive);
                }
            }

            return archive;
        }

        public static bool RemoveArchive(string path)
        {
            Guard.IsNotNullOrWhiteSpace(path, nameof(path));

            if (!archives.Remove(path, out var archive))
                return false;

            archive.Dispose();
            return true;
        }

        public static void ClearArchives()
        {
            lock (archives.SyncRoot)
                archives.SelectValue().DisposeEach(bufferized: false);

            archives.Clear();
        }

        public static void RegisterType(Type type, SnapshotFactory converter)
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsNotNull(converter, nameof(converter));

            lock (convertersSyncRoot)
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

            lock (convertersSyncRoot)
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

        private static void OnArchiveAdd(DictionaryAddEvent<string, SaveArchive> addEv)
        {
            var archivePath = addEv.Key;
            var archive = addEv.Value;

            ObjectRestorer.Archives.Add(archivePath, archive);
        }

        private static void BindArchiveAdd()
        {
            Archives.ObserveDictionaryAdd()
                .Subscribe(OnArchiveAdd);
        }

        private static void OnArchiveRemove(DictionaryRemoveEvent<string, SaveArchive> removeEv)
        {
            var archivePath = removeEv.Key;

            ObjectRestorer.Archives.Remove(archivePath);
        }

        private static void BindArchiveRemove()
        {
            Archives.ObserveDictionaryRemove()
                .Subscribe(OnArchiveRemove);
        }

        private static void OnArchivesClear(Unit _)
        {
            ObjectRestorer.Archives.Clear();
        }

        private static void BindAcrhivesClear()
        {
            Archives.ObserveClear()
                .Subscribe(OnArchivesClear);
        }

        [OnInstallExecutable]
        private static void OnInstall()
        {
            ClearArchives();

            lock (convertersSyncRoot)
                converters.Clear();
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
