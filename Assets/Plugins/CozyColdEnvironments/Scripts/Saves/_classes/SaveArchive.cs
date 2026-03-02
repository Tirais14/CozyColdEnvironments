using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Saves
{
    [Serializable]
    [SerializationDescriptor("SaveArchive", "d619c03c-9b22-4be0-a351-e4cf2e66b4a0")]
    public sealed class SaveArchive
        :
        IEquatable<SaveArchive>,
        IEnumerable<SaveCatalog>
    {
        [JsonProperty("catalogs")]
        private ObservableDictionary<string, SaveCatalog> catalogs = new();

        [JsonIgnore]
        private int? hashCode;

        [JsonIgnore]
        public IReadOnlyObservableDictionary<string, SaveCatalog> Catalogs => catalogs;

        [JsonProperty("path")]
        public string Path { get; init; }

        public SaveArchive(string? path = null)
        {
            Path = path ?? string.Empty;
        }

        public static bool operator ==(SaveArchive? left, SaveArchive? right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Path == right.Path;
        }

        public static bool operator !=(SaveArchive? left, SaveArchive? right)
        {
            return !(left == right);
        }

        public bool RemoveCatalog(string catalogPath, out SaveCatalog? removed)
        {
            Guard.IsNotNull(catalogPath, nameof(catalogPath));

            return catalogs.Remove(catalogPath, out removed);
        }

        public SaveCatalog GetOrCreateCatalog(string path)
        {
            Guard.IsNotNull(path, nameof(path));

            if (!catalogs.TryGetValue(path, out var catalog))
            {
                catalog = new SaveCatalog(this, path);

                catalogs.Add(path, catalog);
            }

            return catalog;
        }

        public SaveArchive Clear()
        {
            catalogs.Clear();

            return this;
        }

        public async UniTask LoadCatalogsFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool force = false,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (catalogs.IsEmpty())
                return;

            await UniTaskHelper.TrySwitchToThreadPool();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadCatalogsFromFileAsync),
                expirationTimeRelativeToNow: TimeSpan.Zero
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, writeSaveDataMode, configureAwait, force))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadCatalogsFromFileAsyncCore(
                        args.writeSaveDataMode,
                        args.force,
                        args.configureAwait,
                        cancellationToken
                        );
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken)
                .ScheduleBy(SaveSystem.CommandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);

            await UniTaskHelper.TrySwitchToMainThread(configureAwait);
        }

        public bool Equals(SaveArchive other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is SaveArchive typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Path);

            return hashCode.Value;
        }

        public override string ToString()
        {
            return $"({nameof(Path)}: {Path})";
        }

        public IEnumerator<SaveCatalog> GetEnumerator()
        {
            return catalogs.To<IDictionary<string, SaveCatalog>>().Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private async UniTask LoadCatalogsFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            bool force = false,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            await UniTaskHelper.TrySwitchToThreadPool();

            using var tasks = new PooledArray<UniTask>(catalogs.Count);

            UniTask task;

            int i = 0;

            try
            {
                lock (catalogs.SyncRoot)
                {
                    foreach (var (_, catalog) in catalogs)
                    {
                        task = catalog.LoadGroupsFromFileAsync(
                            writeSaveDataMode,
                            force,
                            configureAwait: false,
                            cancellationToken: cancellationToken
                            );

                        tasks[i++] = task;
                    }
                }    

                await UniTask.WhenAll(tasks.Raw);
            }
            finally
            {
                await UniTaskHelper.TrySwitchToMainThread(configureAwait);
            }
        }
    }
}
