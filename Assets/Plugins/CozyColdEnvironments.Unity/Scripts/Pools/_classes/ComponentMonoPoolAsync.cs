using CCEnvs.Patterns.Factories;
using CCEnvs.Unity.Patterns.Factory;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Pools
{
    public abstract class ComponentMonoPoolAsync<TComponent, TFactory>
        :
        MonoObjectPoolAsync<TComponent, ComponentPoolAsync<TComponent>, TFactory>

        where TComponent : Component
        where TFactory : Component, IFactory<CancellationToken, ValueTask<TComponent>>
    {
        protected override ComponentPoolAsync<TComponent> CreatePool()
        {
            return new ComponentPoolAsync<TComponent>(factory, capacity: capacity);
        }
    }

    public abstract class ComponentMonoPoolAsync<TComponent> : ComponentMonoPoolAsync<TComponent, UnityObjectMonoFactoryAsync<TComponent>>
        where TComponent : Component
    {

    }

    public class ComponentMonoPoolAsync : ComponentMonoPoolAsync<Component>
    {
    }
}
