#nullable enable
using CCEnvs.Patterns.Factories;

namespace CCEnvs.Pools
{
    public class ObjectPool<T> : AObjectPool<T>, IObjectPool<T>
        where T : class
    {
        private readonly IFactory<T>? factory;

        public override bool HasFactory => factory is not null;

        public ObjectPool(
            IFactory<T>? factory = null,
            int capacity = 4,
            int? maxSize = null)
            :
            base(capacity, maxSize)
        {
            this.factory = factory;
        }

        public PooledHandle<T> Get()
        {
            T obj;

            if (FastObject is not null)
                obj = FastObject;
            else if (InactiveCount <= 0)
            {
                if (factory is null)
                    throw IsEmptyException();

                obj = factory.Create();
            }
            else
                obj = inactiveItems.Dequeue();

            var handle = CreateHandle(obj);
            OnGet(handle);

            return handle;
        }
    }
}
