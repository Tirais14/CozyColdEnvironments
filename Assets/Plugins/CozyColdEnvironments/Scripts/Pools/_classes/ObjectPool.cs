#nullable enable
using CCEnvs.Patterns.Factories;

#pragma warning disable S108
namespace CCEnvs.Pools
{
    public class ObjectPool<T> : ObjectPoolBase<T>, IObjectPool<T>
        where T : class
    {
        private readonly IFactory<T>? factory;

        private readonly bool hasFactory;

        public override bool HasFactory => hasFactory;

        public ObjectPool(
            IFactory<T>? factory = null,
            int capacity = 4,
            int? maxSize = null)
            :
            base(capacity, maxSize)
        {
            this.factory = factory;
            hasFactory = factory.IsNotNull();

        }

        public virtual PooledObject<T> Get()
        {
            T? obj;

            while (!TryGetFromInactive(out obj))
            {
                if (InactiveCount > 0)
                    continue;

                if (!hasFactory)
                    throw PoolEmptyException;

                obj = factory!.Create();

                Return(obj);   
            }

            var handle = CreateHandle(obj);
            GetCore(handle);
            OnGet(handle);

            return handle;
        }
    }
}
