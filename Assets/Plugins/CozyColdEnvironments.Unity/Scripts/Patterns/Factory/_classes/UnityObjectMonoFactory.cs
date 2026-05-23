using CCEnvs.Patterns.Factories;
using CCEnvs.UnityX.Components;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Patterns.Factory
{
    public class UnityObjectMonoFactory<TOut>
        :
        CCBehaviour,
        IFactory<TOut>,
        IFactory<Transform?, TOut>,
        IFactory<InstantiateParameters, TOut>

        where TOut : UnityEngine.Object
    {
        [SerializeField]
        protected TOut prefab;

#if ZENJECT_PLUGIN
        protected Zenject.DiContainer? diContainer;
#endif

        protected UnityObjectFactory<TOut> core { get; private set; } = null!;

#if ZENJECT_PLUGIN
        [Zenject.Inject]
        private void Construct(Zenject.DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }
#endif

        protected override void Start()
        {
            base.Start();
#if ZENJECT_PLUGIN
            core = new UnityObjectFactory<TOut>(prefab, diContainer, cTransform);
#else
            core = new UnityObjectFactory<TOut>(prefab, cTransform);
#endif
        }

        public TOut Create() => core.Create();
        public TOut Create(Transform? parent) => core.Create(parent);
        public TOut Create(InstantiateParameters prms) => core.Create(prms);
    }
}
