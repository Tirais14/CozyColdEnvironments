using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Pools
{
    public class GameObjectPoolAsync : ObjectPoolAsync<GameObject>
    {
        private readonly Transform? objectsRoot;

        public GameObjectPoolAsync(
            IFactory<CancellationToken, ValueTask<GameObject>>? factory = null,
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

        protected override void OnGet(PooledObject<GameObject> handledObj)
        {
            base.OnGet(handledObj);
            handledObj.Value.SetActive(true);
        }

        protected override void OnReturn(GameObject obj)
        {
            base.OnReturn(obj);
            ComponentPool.OnTransfomrReturn(obj.transform);
            obj.transform.SetParent(objectsRoot);
        }
    }
}
