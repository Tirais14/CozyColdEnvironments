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

        where TFactory : MonoBehaviour, IFactory<CancellationToken, ValueTask<GameObject>>
    {
        protected override GameObjectPoolAsync CreatePool()
        {
            return new GameObjectPoolAsync(factory, capacity: capacity);
        }
    }

    public abstract class GameObjectMonoPoolAsync : GameObjectMonoPoolAsync<UnityObjectFactoryAsync<GameObject>>
    {

    }
}
