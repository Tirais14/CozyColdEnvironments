using CCEnvs.Patterns.Factories;
using CCEnvs.Unity.Patterns.Factory;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public abstract class GameObjectMonoPool<TFactory> : MonoObjectPool<GameObject, GameObjectPool, TFactory>
        where TFactory : Component, IFactory<GameObject>
    {
        public override void Return(GameObject? obj)
        {
            base.Return(obj);

            if (obj != null)
                obj.transform.SetParent(cTransform);
        }

        protected override GameObjectPool CreatePool()
        {
            return new GameObjectPool(factory, capacity: capacity);
        }
    }

    public class GameObjectMonoPool : GameObjectMonoPool<UnityObjectMonoFactory<GameObject>>
    {
        protected override GameObjectPool CreatePool()
        {
            return new GameObjectPool(factory, capacity: capacity);
        }
    }
}
