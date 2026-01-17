#nullable enable
using CCEnvs.Patterns.Factories;
using System.Threading.Tasks;

namespace CCEnvs.Pools  
{
    public class ObjectPoolAsync<T> : AObjectPool<T>, IObjectPoolAsync<T>
        where T : class
    {
        private readonly IFactory<ValueTask<T>> factory;

        public ObjectPoolAsync(
            IFactory<ValueTask<T>> factory,
            int capacity, 
            int? maxSize = null)
            :
            base(capacity, maxSize)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
        }

        public ObjectPoolAsync(
            IFactory<ValueTask<T>> factory, 
            int? maxSize = null)
            :
            base(capacity: 4,
                maxSize: maxSize)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
        }

        public async ValueTask<PooledHandle<T>> GetAsync()
        {
            T obj = (InactiveCount <= 0) switch
            {
                true => await factory.Create(),
                _ => inactiveItems.Dequeue()
            };

            var handle = CreateHandle(obj);
            OnGet(handle);

            return handle;
        }
    }
}
