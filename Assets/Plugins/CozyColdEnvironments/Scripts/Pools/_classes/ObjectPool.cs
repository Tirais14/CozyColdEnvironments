#nullable enable
using CCEnvs.Patterns.Factories;

#pragma warning disable S108
namespace CCEnvs.Pools
{
    public class ObjectPool<T> : ObjectPoolBase<T>, IObjectPool<T>
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

        public virtual PooledHandle<T> Get()
        {
            T? obj = null;

            while (!IsObjectValid(obj))
            {
                if (InactiveCount < 1)
                {
                    if (factory is null)
                        throw IsEmptyException();

                    obj = factory.Create();
                    Return(obj);
                }

                TryGetFromInactive(out obj);
            }

            var handle = CreateHandle(obj);
            GetCore(handle);
            OnGet(handle);

            return handle;
        }
    }
}
