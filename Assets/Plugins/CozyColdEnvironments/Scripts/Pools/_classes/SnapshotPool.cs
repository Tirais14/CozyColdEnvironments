using CCEnvs.Snapshots;

#nullable enable
namespace CCEnvs.Pools
{
    public class SnapshotPool<T> : ObjectPool<T>
        where T : class, ISnapshot, new()
    {
        public static SnapshotPool<T> Shared { get; } = new SnapshotPool<T>();

        public SnapshotPool(
            int capacity = 4,
            int? maxSize = null
            )
            :
            base(CCEnvs.Patterns.Factories.Factory.Create(() => new T()),
                capacity: capacity,
                maxSize: maxSize
                )
        {
            
        }

        protected override void OnReturn(T obj)
        {
            base.OnReturn(obj);
            obj.Reset();
        }
    }
}
