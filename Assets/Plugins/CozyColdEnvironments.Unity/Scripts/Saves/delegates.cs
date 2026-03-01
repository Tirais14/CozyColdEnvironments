#nullable enable
using CCEnvs.Snapshots;

namespace CCEnvs.Unity.Saves
{
    public delegate ISnapshot SnapshotFactory(object obj, ISnapshot? existingSnapshot);
}
