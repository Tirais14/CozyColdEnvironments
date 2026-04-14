using CCEnvs.Patterns.Factories;
using CCEnvs.Unity.Components;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public abstract class UnityObjectFactory : CCBehaviour, IFactory
    {
    }

    public abstract class UnityObjectFactory<TOut>
        :
        UnityObjectFactory,
        IFactory<TOut>,
        IFactory<Transform?, TOut>,
        IFactory<InstantiateParameters, TOut>

        where TOut : UnityEngine.Object
    {
        [SerializeField]
        protected TOut prefab;

        public virtual TOut Create()
        {
            return Instantiate(prefab, cTransform);
        }

        public virtual TOut Create(Transform? parent)
        {
            return Instantiate(prefab, parent.IfNull(cTransform));
        }

        public virtual TOut Create(InstantiateParameters prms)
        {
            return Instantiate(prefab.CastTo<UnityEngine.Object>(), prms).CastTo<TOut>();
        }
    }
}
