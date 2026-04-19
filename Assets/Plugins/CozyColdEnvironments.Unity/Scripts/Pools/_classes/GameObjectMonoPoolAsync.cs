using CCEnvs.Patterns.Factories;
using CCEnvs.Unity.Patterns.Factory;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CCEnvs.Unity.Pools
{
    public abstract class GameObjectMonoPoolAsync<TFactory>
        :
        MonoObjectPoolAsync<GameObject, GameObjectPoolAsync, TFactory>

        where TFactory : Component, IFactory<CancellationToken, ValueTask<GameObject>>
    {
        protected override GameObjectPoolAsync CreatePool()
        {
            return new GameObjectPoolAsync(factory, capacity: capacity);
        }
    }

    public abstract class GameObjectMonoPoolAsync : GameObjectMonoPoolAsync<UnityObjectMonoFactoryAsync<GameObject>>
    {

    }
}
