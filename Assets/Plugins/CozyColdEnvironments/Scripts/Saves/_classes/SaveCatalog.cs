using CCEnvs.Diagnostics;
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

        public SaveGroup this[string groupName] {
            get
            {
                if (groups.TryGetValue(groupName, out var group))
                    return group;

                if (incrementalGroups.TryGetValue(groupName, out var incGroup))
                    return incGroup;

                throw new InvalidOperationException($"Cannot find group. Group: {groupName}");
            }
        }

        public SaveGroupIncremental this[string groupName, bool _] {
            get => incrementalGroups[groupName];
        }

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
            CreateGroupParameters createParams
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotDefault(createParams, nameof(createParams));

            SaveGroup? group;

            lock (groups.SyncRoot)
            {
                if (!groups.TryGetValue(createParams.GroupName, out group))
                {
                    if (incrementalGroups.TryGetValue(createParams.GroupName, out var incGroup))
                    {
                        if (CCDebug.Instance.IsEnabled)
                            this.PrintWarning($"Group already exists as incremental. Group: {createParams.GroupName}");

                        group = incGroup;
                    }
                    else
                    {
                        group = new SaveGroup(
                            this,
                            createParams.GroupName,
                            saveDataVersion: createParams.SaveDataVersion,
                            redirectionMode: createParams.Redirection,
                            loadOnFirstObjectRegistered: createParams.LoadOnFirstObjectRegistered
                            );

                        groups.Add(createParams.GroupName, group);
                    }
                }
            }

            if (group.SaveData.Version != createParams.SaveDataVersion)
                PrintVersionsNotMatchWarning(group.SaveData.Version, createParams.SaveDataVersion);

            if (group.Redirection != createParams.Redirection)
                PrintRedirectionModesNotMatchError(group.Redirection, createParams.Redirection);

            return group;
        }

        public SaveGroup[] GetOrCreateGroups(
            CreateGroupParameters? createParamsBase,
            params string[] groupNames
            )
        {
            Guard.IsNotNull(groupNames, nameof(groupNames));

            var groups = new SaveGroup[groupNames.Length];

            CreateGroupParameters createParams;

            string groupName;

            for (int i = 0; i < groupNames.Length; i++)
            {
                groupName = groupNames[i];

                createParams = createParamsBase.GetValueOrDefault().WithGroupName(groupName);

                groups[i] = GetOrCreateGroup(createParams);
            }

            return groups;
        }

        public SaveGroupIncremental GetOrCreateIncrementalGroup(
            CreateGroupParameters createParams
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotDefault(createParams, nameof(createParams));

            SaveGroupIncremental? group;

            lock (incrementalGroups.SyncRoot)
            {
                if (!incrementalGroups.TryGetValue(createParams.GroupName, out group))
                {
                    if (groups.ContainsKey(createParams.GroupName))
                        throw new InvalidOperationException($"Group already exists as basic. Group: {createParams.GroupName}");

                    group = new SaveGroupIncremental(
                        this,
                        createParams.GroupName,
                        saveDataVersion: createParams.SaveDataVersion,
                        redirectionMode: createParams.Redirection,
                        loadOnFirstObjectRegistered: createParams.LoadOnFirstObjectRegistered
                        );

                    incrementalGroups.Add(createParams.GroupName, group);
                }
            }

            if (group.SaveData.Version != createParams.SaveDataVersion)
                PrintVersionsNotMatchWarning(group.SaveData.Version, createParams.SaveDataVersion);

            if (group.Redirection != createParams.Redirection)
                PrintRedirectionModesNotMatchError(group.Redirection, createParams.Redirection);

            return group;
        }

        public SaveGroupIncremental[] GetOrCreateIncrementalGroups(
            CreateGroupParameters? createParamsBase,
            params string[] groupNames
            )
        {
            Guard.IsNotNull(groupNames, nameof(groupNames));

            var incGroups = new SaveGroupIncremental[groupNames.Length];

            CreateGroupParameters createParams;

            string groupName;

            for (int i = 0; i < groupNames.Length; i++)
            {
                groupName = groupNames[i];

                createParams = createParamsBase.GetValueOrDefault().WithGroupName(groupName);

                incGroups[i] = GetOrCreateIncrementalGroup(createParams);
            }

            return incGroups;
        }

        public bool ChangeGroupTypeTo(
            string groupName,
            bool toIncremental
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotNull(groupName, nameof(groupName));

            bool isGroupIncremental = incrementalGroups.Remove(groupName, out var incGroup);
            bool isBasicGroup = groups.Remove(groupName, out var group);
            bool success = false;

            try
            {
                if (isGroupIncremental && !toIncremental)
                {
                    group = SaveGroup.ConvertToNonIncremental(incGroup);
                    success = true;

                    groups[groupName] = group;
                }
                else if (isBasicGroup && toIncremental)
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
            CreateGroupParameters createParams,
            bool incremental
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);
            Guard.IsNotDefault(createParams, nameof(createParams));

            if (incremental)
            {
                if (incrementalGroups.TryGetValue(createParams.GroupName, out var incGroup))
                    return incGroup;

                if (ChangeGroupTypeTo(createParams.GroupName, incremental))
                    return incrementalGroups[createParams.GroupName];

                return GetOrCreateIncrementalGroup(createParams);
            }

            if (groups.TryGetValue(createParams.GroupName, out var group))
                return group;

            if (ChangeGroupTypeTo(createParams.GroupName, incremental))
                return groups[createParams.GroupName];

            return GetOrCreateIncrementalGroup(createParams);
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
