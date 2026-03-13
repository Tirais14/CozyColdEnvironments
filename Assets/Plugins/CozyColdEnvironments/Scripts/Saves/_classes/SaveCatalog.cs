using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Patterns.Commands;
using CommunityToolkit.Diagnostics;
using ObservableCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Saves
{
    public sealed class SaveCatalog
        :
        IEnumerable<SaveGroup>,
        IDisposable
    {
        internal CommandScheduler commandScheduler = CommandScheduler.CreateDefaultRegistered(nameof(SaveCatalog));

        private ObservableDictionary<string, SaveGroup> groups = new();
        private ObservableDictionary<string, SaveGroupIncremental> incrementalGroups = new();

        private int? hashCode;

        public IReadOnlyObservableDictionary<string, SaveGroup> Groups => groups;

        public IReadOnlyObservableDictionary<string, SaveGroupIncremental> IncrementalGroups => incrementalGroups;

        public string Path { get; }

        public SaveArchive Archive { get;}

        public SaveCatalogLoader Loader { get; }

        public SaveCatalogSerializer Serializer { get; }

        public SaveCatalogWriter Writer { get; }

        public SaveCatalog(
            SaveArchive archive,
            string? path = null
            )
        {
            Guard.IsNotNull(archive, nameof(archive));

            Path = path ?? string.Empty;
            Archive = archive;
            Loader = new SaveCatalogLoader(this);
            Serializer = new SaveCatalogSerializer(this);
            Writer = new SaveCatalogWriter(this);
        }

        ~SaveCatalog() => Dispose();

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

        public SaveGroup GetOrCreateGroup(
            string groupName,
            long saveDataVersion = 0L,
            RedirectionMode redirectionMode = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotNull(groupName, nameof(groupName));

            SaveGroup? group;

            lock (groups.SyncRoot)
            {
                if (!groups.TryGetValue(groupName, out group))
                {
                    if (incrementalGroups.ContainsKey(groupName))
                        throw new InvalidOperationException($"Group: {groupName} already exists and it's incremental");

                    group = new SaveGroup(
                        this,
                        groupName,
                        saveDataVersion,
                        redirectionMode
                        );

                    groups.Add(groupName, group);
                }
            }

            if (group.SaveData.Version != saveDataVersion)
                PrintVersionsNotMatchWarning(group.SaveData.Version, saveDataVersion);

            if (group.Redirection != redirectionMode)
                PrintRedirectionModesNotMatchError(group.Redirection, redirectionMode);

            return group;
        }

        public SaveGroupIncremental GetOrCreateIncrementalGroup(
            string groupName, 
            long saveDataVersion = 0L,
            RedirectionMode redirectionMode = default
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotNull(groupName, nameof(groupName));

            SaveGroupIncremental? group;

            lock (incrementalGroups.SyncRoot)
            {
                if (!incrementalGroups.TryGetValue(groupName, out group))
                {
                    if (groups.ContainsKey(groupName))
                        throw new InvalidOperationException($"Group: {groupName} already exists and it's not incremental");

                    group = new SaveGroupIncremental(
                        this,
                        groupName,
                        saveDataVersion,
                        redirectionMode
                        );

                    incrementalGroups.Add(groupName, group);
                }
            }

            if (group.SaveData.Version != saveDataVersion)
                PrintVersionsNotMatchWarning(group.SaveData.Version, saveDataVersion);

            if (group.Redirection != redirectionMode)
                PrintRedirectionModesNotMatchError(group.Redirection, redirectionMode);

            return group;
        }

        public bool ChangeGroupTypeTo(
            string groupName,
            bool incremental
            )
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

        public override string ToString()
        {
            return $"({nameof(Path)}: {Path}; {nameof(Archive)}: {Archive})";
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            Loader.Dispose();
            Writer.Dispose();

            lock (groups.SyncRoot)
                groups.SelectValue().DisposeEach(bufferized: false);

            groups.Clear();

            lock (incrementalGroups.SyncRoot)
                incrementalGroups.SelectValue().DisposeEach(bufferized: false);

            incrementalGroups.Clear();

            GC.SuppressFinalize(this);
        }

        public IEnumerator<SaveGroup> GetEnumerator()
        {
            return groups.To<IDictionary<string, SaveGroup>>().Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void PrintVersionsNotMatchWarning(long current, long expected)
        {
            this.PrintWarning($"Versions is not match. Current: {current}; expected: {expected}");
        }

        private void PrintRedirectionModesNotMatchError(RedirectionMode current, RedirectionMode expected)
        {
            this.PrintError($"Redirection modes is not match. Current: {current}; expected: {expected}");
        }
    }
}
