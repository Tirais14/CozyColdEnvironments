#nullable enable
using CCEnvs.Snapshots;

namespace CCEnvs.Saves
{
    public delegate ISnapshot SnapshotFactory(object obj);

    public delegate ISnapshot SnapshotFactory<in T>(T obj);

    public delegate void OnSaveObjectIsDirtyChanged(ISaveObjectIncremental obj, bool state);

    public delegate SaveEntry SaveEntryVersionUpgrader(SaveEntry entry, long nextVersion);
}
