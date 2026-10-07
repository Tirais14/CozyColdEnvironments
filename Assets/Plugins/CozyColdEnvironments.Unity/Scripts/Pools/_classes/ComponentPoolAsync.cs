using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Pools
{
    public class ComponentPoolAsync<T> : ObjectPoolAsync<T>
        where T : Component
    {
        private readonly Transform? objectsRoot;

        public ComponentPoolAsync(
            IFactory<CancellationToken, ValueTask<T>>? factory = null,
            int capacity = 4,
            int? maxSize = null,
            Transform? objectsRoot = null
            )
            :
            base(factory: factory,
                capacity: capacity,
                maxSize: maxSize
        )
        {
            this.objectsRoot = objectsRoot;
        }

        protected override void OnGet(PooledObject<T> handledObj)
        {
            base.OnGet(handledObj);
            handledObj.Value.gameObject.SetActive(true);
        }

        protected override void OnReturn(T obj)
        {
            base.OnReturn(obj);
            ComponentPool.OnTransfomrReturn(obj.transform);
            obj.transform.SetParent(objectsRoot);
        }
    }
}
