using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public class SaveGroupCatalog
    {
        private readonly Dictionary<(string groupName, string? groupID), SaveGroup> saveGroups = new(0);

        public string Path { get; }

        public IReadOnlyDictionary<(string groupName, string? groupID), SaveGroup> SaveGroups => saveGroups;

        public SaveGroupCatalog(string path)
        {
            Path = path;
        }

        public SaveGroup GetOrCreateSaveGroup(string groupName, string? groupID = null)
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            if (!saveGroups.TryGetValue((groupName, groupID), out var group))
            {
                group = new SaveGroup(groupName, groupID);

                saveGroups.Add((groupName, groupID), group);
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
