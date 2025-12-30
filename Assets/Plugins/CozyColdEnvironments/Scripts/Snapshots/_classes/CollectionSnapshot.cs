using System.Collections;

#nullable enable
namespace CCEnvs.Snapshots
{
    public abstract class CollectionSnapshot<T> : Snapshot<T>
        where T : ICollection
    {
        protected CollectionSnapshot()
        {
        }

        protected CollectionSnapshot(T target) : base(target)
        {
        }
    }
}
