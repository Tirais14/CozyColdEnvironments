using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Saves
{
    [SerializationDescriptor("SaveCatalog", "f6d4d3d5-bfab-4d7a-89a8-2107c8b2d497")]
    public sealed class SaveCatalog
        :
        IEquatable<SaveCatalog>,
        IEnumerable<SaveGroup>,
        IDisposable
    {
        private ObservableDictionary<string, SaveGroup> groups = new();

        private ObservableDictionary<string, SaveGroupIncremental> incrementalGroups = new();

        private int? hashCode;

        public IReadOnlyObservableDictionary<string, SaveGroup> Groups => groups;

        public string Path { get; init; }

        public SaveArchive Archive { get; init; }

        public SaveCatalog(
            SaveArchive archive,
            string? path = null
            )
        {
            Guard.IsNotNull(archive, nameof(archive));

            Path = path ?? string.Empty;
            Archive = archive;
        }

        public static bool operator ==(SaveCatalog? left, SaveCatalog? right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Path == right.Path
                   &&
                   left.Archive == right.Archive
                   &&
                   left.disposed == right.disposed;
        }

        public static bool operator !=(SaveCatalog? left, SaveCatalog? right)
        {
            return !(left == right);
        }

        public bool RemoveGroup(string groupName)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotNull(groupName, nameof(groupName));

            if (incrementalGroups.Remove(groupName, out var incGroup))
            {
                incGroup.Dispose();
                return true;
            }

            if (groups.Remove(groupName, out var group))
            {
                group.Dispose();
                return true;
            }

            return false;
        }

        public SaveGroup GetOrCreateGroup(string groupName)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotNull(groupName, nameof(groupName));

            if (!groups.TryGetValue(groupName, out var group))
            {
                if (incrementalGroups.ContainsKey(groupName))
                    throw new InvalidOperationException($"Group: {groupName} already exists and it's incremental");

                group = new SaveGroup(this, groupName);

                groups.Add(groupName, group);
            }

            return group;
        }

        public SaveGroupIncremental GetOrCreateIncrementalGroup(string groupName)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotNull(groupName, nameof(groupName));

            if (!incrementalGroups.TryGetValue(groupName, out var group))
            {
                if (groups.ContainsKey(groupName))
                    throw new InvalidOperationException($"Group: {groupName} already exists and it's not incremental");

                group = new SaveGroupIncremental(this, groupName);

                incrementalGroups.Add(groupName, group);
            }

            return group;
        }

        public bool ChangeGroupTypeTo(string groupName, bool incremental)
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotNull(groupName, nameof(groupName));

            bool isGroupIncremental = incrementalGroups.Remove(groupName, out var incGroup);
            bool isBasicGroup = groups.Remove(groupName, out var group);
            bool success = false;

            try
            {
                if (isGroupIncremental && !incremental)
                {
                    group = SaveGroup.ConvertToNonIncremental(incGroup);
                    success = true;

                    groups[groupName] = group;
                }
                else if (isBasicGroup && incremental)
                {
                    incGroup = SaveGroup.ConvertToIncremental(group);
                    success = true;

                    incrementalGroups[groupName] = incGroup;
                }

            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                if (isGroupIncremental)
                    incrementalGroups.Add(groupName, incGroup);
                else if (isBasicGroup)
                    groups.Add(groupName, group);
            }

            return success;
        }

        public SaveGroup GetOrCreateGroupGeneric(
            string groupName,
            bool incremental
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotNull(groupName, nameof(groupName));

            if (incremental)
            {
                if (incrementalGroups.TryGetValue(groupName, out var incGroup))
                    return incGroup;

                if (ChangeGroupTypeTo(groupName, incremental))
                    return incrementalGroups[groupName];

                return GetOrCreateIncrementalGroup(groupName);
            }

            if (groups.TryGetValue(groupName, out var group))
                return group;

            if (ChangeGroupTypeTo(groupName, incremental))
                return groups[groupName];

            return GetOrCreateIncrementalGroup(groupName);
        }

        public SaveCatalog Clear()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            lock (groups.SyncRoot)
                groups.SelectValue().DisposeEach(bufferized: false);

            lock (incrementalGroups.SyncRoot)
                incrementalGroups.SelectValue().DisposeEach(bufferized: false);

            groups.Clear();
            incrementalGroups.Clear();

            return this;
        }

        public string GetFullPath()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            return System.IO.Path.Join(Archive.Path, Path);
        }

        public async UniTask LoadGroupsFromFileAsync(
            WriteSaveDataMode writeSaveDataMode = default,
            bool force = false,
            bool configureAwait = true,
            CancellationToken cancellationToken = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            cancellationToken.ThrowIfCancellationRequested();

            if (groups.IsEmpty())
                return;

            await UniTaskHelper.TrySwitchToThreadPool();

            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(LoadGroupsFromFileAsync),
                expirationTimeRelativeToNow: TimeSpan.Zero
                );

            await Command.Builder.WithName(cmdName)
                .WithState((@this: this, writeSaveDataMode, configureAwait, force))
                .Asynchronously()
                .WithExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await args.@this.LoadGroupsFromFileAsyncCore(
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

        public bool Equals(SaveCatalog other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is SaveCatalog typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Path, Archive, disposed);

            return hashCode.Value;
        }

        public override string ToString()
        {
            return $"({nameof(Path)}: {Path}; {nameof(Archive)}: {Archive})";
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            groups.SelectValue().DisposeEach(bufferized: false);
            groups.Clear();

            incrementalGroups.SelectValue().DisposeEach(bufferized: false);
            incrementalGroups.Clear();

            GC.SuppressFinalize(this);
        }

        public IEnumerator<SaveGroup> GetEnumerator()
        {
            return groups.To<IDictionary<string, SaveGroup>>().Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private async UniTask LoadGroupsFromFileAsyncCore(
            WriteSaveDataMode writeSaveDataMode = default,
            bool configureAwait = true,
            bool force = false,
            CancellationToken cancellationToken = default
            )
        {
            await UniTaskHelper.TrySwitchToThreadPool();

            using var tasks = new PooledArray<UniTask>(groups.Count);

            UniTask task;

            int i = 0;

            try
            {
                lock (groups.SyncRoot)
                {
                    foreach (var (_, group) in groups)
                    {
                        task = group.LoadSaveDataFromFileAsync(
                            writeSaveDataMode,
                            configureAwait: false,
                            force,
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
