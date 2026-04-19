using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using CCEnvs.TypeMatching;
using UnityEngine;
using UnityEngine.Splines;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public abstract class UnityObjectFactory
    {
        protected readonly Transform? defaultRoot;

#if ZENJECT_PLUGIN
        protected readonly Zenject.DiContainer? diContainer;
#endif

        public UnityObjectFactory()
        {
        }

        public UnityObjectFactory(Transform? defaultRoot)
        {
            this.defaultRoot = defaultRoot;
        }

#if ZENJECT_PLUGIN
        public UnityObjectFactory(Transform? defaultRoot, Zenject.DiContainer? diContainer)
            :
            this(defaultRoot)
        {
            this.diContainer = diContainer;
        }
#endif

        protected virtual void InjectInstance(Object instance)
        {
#if ZENJECT_PLUGIN
            if (diContainer is not null)
            {
                using var cmps = new PooledList<MonoBehaviour>(32);

                if (instance.Is<Component>(out var cmp))
                    cmp.GetComponentsInChildren<MonoBehaviour>(includeInactive: true, cmps);
                else if (instance.Is<GameObject>(out var go))
                    go.GetComponentsInChildren<MonoBehaviour>(includeInactive: true, cmps);

                MonoBehaviour monoBeh;

                for (int i = 0; i < cmps.Count; i++)
                {
                    monoBeh = cmps[i];
                    diContainer.Inject(monoBeh);
                }
            }
#endif
        }
    }

    public class UnityObjectFactory<TOut>
        :
        UnityObjectFactory,
        IFactory<TOut>,
        IFactory<Transform?, TOut>,
        IFactory<InstantiateParameters, TOut>

        where TOut : Object
    {
        protected readonly TOut prefab;

        public UnityObjectFactory(TOut prefab, Transform? defaultRoot = null)
            :
            base(defaultRoot)
        {
            CC.Guard.IsNotNull(prefab, nameof(prefab));

            this.prefab = prefab;
        }

#if ZENJECT_PLUGIN
        public UnityObjectFactory(
            TOut prefab,
            Zenject.DiContainer? diContainer,
            Transform? defaultRoot = null
            )
            :
            base(defaultRoot, diContainer)
        {
            CC.Guard.IsNotNull(prefab, nameof(prefab));

            this.prefab = prefab;
        }
#endif

        public virtual TOut Create()
        {
            var instance = Object.Instantiate(prefab, defaultRoot);

#if ZENJECT_PLUGIN
            InjectInstance(instance);
#endif

            return instance;
        }

        public virtual TOut Create(Transform? parent)
        {
            var instance = Object.Instantiate(prefab, parent.IfNull(defaultRoot));

#if ZENJECT_PLUGIN
            InjectInstance(instance);
#endif
            return instance;
        }

        public virtual TOut Create(InstantiateParameters prms)
        {
            var instance = Object.Instantiate(prefab, prms);

#if ZENJECT_PLUGIN
            InjectInstance(instance);
#endif

            return instance;
        }
    }
}
