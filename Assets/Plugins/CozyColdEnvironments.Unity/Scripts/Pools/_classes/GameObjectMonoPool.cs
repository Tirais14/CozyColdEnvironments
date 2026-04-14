using CCEnvs.Unity.Patterns.Factory;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public abstract class GameObjectMonoPool<TFactory> : MonoObjectPool<GameObject, GameObjectPool, TFactory>
        where TFactory : UnityObjectFactory<GameObject>
    {
        protected override GameObjectPool CreatePool()
        {
            return new GameObjectPool(factory, capacity: capacity);
        }
    }

    public class GameObjectMonoPool : GameObjectMonoPool<UnityObjectFactory<GameObject>>
    {
        protected override GameObjectPool CreatePool()
        {
            return new GameObjectPool(factory, capacity: capacity);
        }
    }
}
