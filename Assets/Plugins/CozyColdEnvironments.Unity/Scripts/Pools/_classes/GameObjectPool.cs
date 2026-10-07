using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using System;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Pools
{
    public class GameObjectPool : ObjectPool<GameObject>
    {
        private static readonly Lazy<GameObjectPool> shared = new(() => new());

        private readonly Transform? objectsRoot;

        public static GameObjectPool Shared => shared.Value;

        public GameObjectPool(
            IFactory<GameObject>? factory = null,
            int capacity = 4,
            int? maxSize = null,
            Transform? objectRoot = null
            )
            :
            base(factory: factory,
                capacity: capacity,
                maxSize: maxSize
                )
        {
            this.objectsRoot = objectRoot;
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

        private int disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
            {
                if (fastObject != null)
                    UnityEngine.Object.Destroy(fastObject);

                while (inactiveItems.TryPop(out var go))
                    UnityEngine.Object.Destroy(go);
            }
        }
    }
}
