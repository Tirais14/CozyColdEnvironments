using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks.Triggers;
using ObservableCollections;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public class SaveGroupCatalog : IEnumerable<KeyValuePair<string, SaveGroup>>
    {
        private readonly ObservableDictionary<string, SaveGroup> groups = new();

        public IReadOnlyObservableDictionary<string, SaveGroup> Groups => groups;

        public string Path { get; }

        public SaveArchive Archive { get; }

        public SaveGroupCatalog(
            SaveArchive archive,
            string? path = null
            )
        {
            Guard.IsNotNull(archive, nameof(archive));

            Path = path ?? string.Empty;
            Archive = archive;
        }

        //public void AddGroup(SaveGroup group)
        //{
        //    Guard.IsNotNull(group, nameof(group));

        //    groups.Add(group.Name, group);
        //}

        public bool RemoveGroup(string groupName, out SaveGroup? removed)
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            return groups.Remove(groupName, out removed);
        }

        public SaveGroup GetOrCreateGroup(string groupName)
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            if (!groups.TryGetValue(groupName, out var group))
            {
                group = new SaveGroup(this, groupName);

                groups.Add(groupName, group);
            }

            return group;
        }

        public bool ContainsSaveGroup(string groupName)
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            return groups.ContainsKey(groupName);
        }

        public IEnumerator<KeyValuePair<string, SaveGroup>> GetEnumerator()
        {
            return groups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
