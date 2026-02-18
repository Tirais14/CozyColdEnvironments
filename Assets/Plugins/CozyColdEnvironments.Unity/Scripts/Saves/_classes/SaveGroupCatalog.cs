using CommunityToolkit.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public class SaveGroupCatalog
    {
        private readonly ConcurrentDictionary<(string groupName, string? groupID), SaveGroup> saveGroups = new();

        public string Path { get; }

        public IReadOnlyDictionary<(string groupName, string? groupID), SaveGroup> SaveGroups => saveGroups;

        public SaveGroupCatalog(string path)
        {
            Path = path;
        }

        public SaveGroup GetOrCreateSaveGroup(string groupName, string? groupID = null)
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            var groupKey = (groupName, groupID);

            if (!saveGroups.TryGetValue(groupKey, out var group))
            {
                group = new SaveGroup(groupName, groupID);

                if (!saveGroups.TryAdd(groupKey, group))
                    group = saveGroups[groupKey];
            }

            return group;
        }

        public bool ContainsSaveGroup(string groupName, string? groupID = null)
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            return saveGroups.ContainsKey((groupName, groupID));
        }
    }
}
