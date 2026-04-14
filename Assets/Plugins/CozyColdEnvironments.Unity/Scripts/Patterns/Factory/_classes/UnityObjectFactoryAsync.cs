using CCEnvs.Patterns.Factories;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public abstract class UnityObjectFactoryAsync<TOut>
        :
        UnityObjectFactory,
        IFactory<CancellationToken, ValueTask<TOut>>,
        IFactory<Transform?, CancellationToken, ValueTask<TOut>>,
        IFactory<InstantiateParameters, CancellationToken, ValueTask<TOut>>,
        IFactory<int, CancellationToken, ValueTask<TOut[]>>,
        IFactory<int, Transform?, CancellationToken, ValueTask<TOut[]>>,
        IFactory<int, InstantiateParameters, CancellationToken, ValueTask<TOut[]>>

        where TOut : UnityEngine.Object
    {
        [SerializeField]
        protected TOut prefab;

        public virtual async ValueTask<TOut> Create(CancellationToken cancellationToken)
        {
            return (await InstantiateAsync(prefab, cTransform))[0];
        }

        public virtual async ValueTask<TOut> Create(Transform? parent, CancellationToken cancellationToken)
        {
            return (await InstantiateAsync(prefab, parent.IfNull(cTransform)))[0];
        }

        public virtual async ValueTask<TOut> Create(InstantiateParameters prms, CancellationToken cancellationToken)
        {
            return (await InstantiateAsync(prefab, prms))[0];
        }

        public virtual async ValueTask<TOut[]> Create(int count, CancellationToken cancellationToken)
        {
            return await InstantiateAsync(prefab, count, cTransform);
        }

        public virtual async ValueTask<TOut[]> Create(int count, Transform? parent, CancellationToken cancellationToken)
        {
            return await InstantiateAsync(prefab, count, parent.IfNull(cTransform));
        }

        public virtual async ValueTask<TOut[]> Create(int count, InstantiateParameters prms, CancellationToken cancellationToken)
        {
            return await InstantiateAsync(prefab, count, prms);
        }
    }
}
