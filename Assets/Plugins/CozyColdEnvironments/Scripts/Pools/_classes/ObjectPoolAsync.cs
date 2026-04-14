#nullable enable
using System.Threading;
using System.Threading.Tasks;
using CCEnvs.Patterns.Factories;

namespace CCEnvs.Pools
{
    public class ObjectPoolAsync<T> : ObjectPoolBase<T>, IObjectPoolAsync<T>
        where T : class
    {
        private readonly IFactory<CancellationToken, ValueTask<T>>? factory;

        private readonly bool hasFactory;

        public override bool HasFactory => hasFactory;

        public ObjectPoolAsync(

            IFactory<CancellationToken, ValueTask<T>>? factory = null,
            int capacity = 4,
            int? maxSize = null)
            :
            base(capacity, maxSize)
        {
            this.factory = factory;
            hasFactory = factory.IsNotNull();
        }

        public async ValueTask<PooledObject<T>> GetAsync(CancellationToken cancellationToken = default)
        {
            T? obj;

            while (!TryGetFromInactive(out obj))
            {
                if (InactiveCount > 0)
                    continue;

                if (!HasFactory)
                    throw PoolEmptyException;

                obj = await factory!.Create(cancellationToken);

                Return(obj);
            }

            var handle = CreateHandle(obj);
            GetCore(handle);
            OnGet(handle);

            return handle;
        }
    }
}
