#nullable enable
using CCEnvs.Snapshots;

namespace CCEnvs.Unity.Saves
{
    public delegate ISnapshot SnapshotFactory(object obj);

    public delegate ISnapshot SnapshotFactory<T>(T obj);

    public delegate void OnSaveObjectIsDirtyChanged(ISaveObjectIncremental obj, bool state);
}
