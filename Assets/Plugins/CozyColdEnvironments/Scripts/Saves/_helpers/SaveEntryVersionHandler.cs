using System.Collections.Generic;
using CCEnvs.Disposables;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Saves
{
    public static class SaveEntryVersionHandler
    {
        private static readonly Dictionary<SaveEntryUpgradeVersionInfo, SaveEntryVersionUpgrader> upgraders = new();

        public static IReadOnlyDictionary<SaveEntryUpgradeVersionInfo, SaveEntryVersionUpgrader> Upgraders => upgraders;

        public static void RegisterUpgrader(
            SaveEntryUpgradeVersionInfo info,
            SaveEntryVersionUpgrader upgrader
            )
        {
            Guard.IsNotDefault(info, nameof(info));
            Guard.IsNotNull(upgrader, nameof(upgrader));

            upgraders[info] = upgrader;
        }

        public static SaveEntryUpgraderRegistration RegisterDisposableHandled(
            SaveEntryUpgradeVersionInfo info,
            SaveEntryVersionUpgrader upgrader
            )
        {
            RegisterDisposableHandled(info, upgrader);

            return new SaveEntryUpgraderRegistration(info);
        }

        public static bool UnregisterUpgrader(SaveEntryUpgradeVersionInfo info)
        {
            return upgraders.Remove(info);
        }

        public static bool TryUpgrade(
            SaveEntry entry,
            long nextVersion,
            out SaveEntry upgradedEntry
            )
        {
            Guard.IsNotDefault(entry, nameof(entry));

            var entryInfo = new SaveEntryUpgradeVersionInfo(entry.Snapshot.GetType(), entry.Version, nextVersion);

            if (!upgraders.TryGetValue(entryInfo, out var upgrader))
            {
                upgradedEntry = entry;
                return false;
            }

            try
            {
                upgradedEntry = upgrader(entry, nextVersion);
                return true;
            }
            catch (System.Exception ex)
            {
                typeof(SaveEntryVersionHandler).PrintException(ex);

                upgradedEntry = entry;
                return false;
            }
        }
    }
}
