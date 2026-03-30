using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Json;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.Reflection;
using CCEnvs.Saves.Json;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ObservableCollections;
using R3;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

#nullable enable
namespace CCEnvs.Saves
{
    public static partial class SaveSystem
    {
        public const int MAX_IO_OPERATIONS =
#if PLATFORM_WEBGL
            1;
#else
            2;
#endif

        private readonly static ConcurrentDictionary<Type, SnapshotFactory> converters = new();

        private readonly static object convertersGate = new();

        private readonly static ObservableDictionary<string, SaveArchive> archives = new(1, null);

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

        internal static SemaphoreSlim IOSemaphore { get; } = new(MAX_IO_OPERATIONS);

        internal static SemaphoreSlim SerializingSemaphore { get; } = new(Math.Clamp(Environment.ProcessorCount / 2, 1, int.MaxValue));

        internal static CommandScheduler CommandScheduler { get; } = CommandScheduler.CreateDefaultRegistered();

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

        public static SaveArchive[] GetOrCreateArchives(params string[] paths)
        {
            Guard.IsNotNull(paths, nameof(paths));

            var archives = new SaveArchive[paths.Length];

            for (int i = 0; i < paths.Length; i++)
                archives[i] = GetOrCreateArchive(paths[i]);

            return archives;
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

            return converters.TryRemove(type, out _);
        }

        public static bool UnregisterType<T>()
        {
            return UnregisterType(typeof(T));
        }

        public static SnapshotFactory ResolveConverter(Type type)
        {
            Guard.IsNotNull(type, nameof(type));

            if (!converters.TryGetValue(type, out var converter))
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

            serializerSettings.AddConverters(new SaveEntriesJsonConverter());

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

            ObjectRestorer.ClearArchives();

            lock (convertersGate)
                converters.Clear();
        }

        private readonly static Stack<object[]> registerByAttributesBuffers = new();

        [OnInstallExecutable]
        private static void RegisterTypeByAttributes(MemberInfo[] domainMembers)
        {
            (from member in domainMembers.AsParallel()
             where member.MemberType.HasFlagT(MemberTypes.TypeInfo) || member.MemberType.HasFlagT(MemberTypes.NestedType)
             select (Type)member into type
             where type.IsDefined<RegisterInSaveSystemAttribute>(inherit: false)
             select type
             )
             .ForAll(
                static type =>
                {
                    try
                    {
                        var ctor = Snapshot.GetConstructorByAttribute(type);

                        var factory = CreateSnapshotFactory(ctor);

                        RegisterType(type, factory);
                    }
                    catch (Exception ex)
                    {
                        typeof(SaveSystem).PrintException(ex);
                    }
                });
        }

        private static SnapshotFactory CreateSnapshotFactory(ConstructorInfo ctor)
        {
            return (obj) =>
            {
                if (!registerByAttributesBuffers.TryPop(out var buffer))
                    buffer = new object[0];

                try
                {
                    buffer[0] = obj;
                    return (ISnapshot)ctor.Invoke(buffer);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    buffer[0] = null!;
                    registerByAttributesBuffers.Push(buffer);
                }
            };
        }
    }
}
