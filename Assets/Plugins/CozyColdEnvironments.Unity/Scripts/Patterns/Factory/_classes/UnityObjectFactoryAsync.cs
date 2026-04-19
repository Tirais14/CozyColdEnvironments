using CCEnvs.Patterns.Factories;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.Patterns.Factory
{
    public class UnityObjectFactoryAsync<TOut>
        :
        UnityObjectFactory,
        IFactory<CancellationToken, ValueTask<TOut>>,
        IFactory<Transform?, CancellationToken, ValueTask<TOut>>,
        IFactory<InstantiateParameters, CancellationToken, ValueTask<TOut>>,
        IFactory<int, CancellationToken, ValueTask<TOut[]>>,
        IFactory<int, Transform?, CancellationToken, ValueTask<TOut[]>>,
        IFactory<int, InstantiateParameters, CancellationToken, ValueTask<TOut[]>>

        where TOut : Object
    {
        protected readonly TOut prefab = null!;

        public UnityObjectFactoryAsync(TOut prefab, Transform? defaultRoot = null)
            :
            base(defaultRoot)
        {
            CC.Guard.IsNotNull(prefab, nameof(prefab));

            this.prefab = prefab;
        }

#if ZENJECT_PLUGIN
        public UnityObjectFactoryAsync(
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

        public virtual async ValueTask<TOut> Create(CancellationToken cancellationToken)
        {
            var activeScene = SceneManagerHelper.ActiveScene;
            var instance = (await Object.InstantiateAsync(prefab, new InstantiateParameters() { parent = defaultRoot }, cancellationToken: cancellationToken))[0];

            if (!activeScene.IsValid()
#if UNITY_EDITOR
                || 
                !Application.isPlaying
#endif
                )
            {
                instance.DestroyByGameObject();
                throw new InvalidOperationException();
            }

#if ZENJECT_PLUGIN
            InjectInstance(instance);
#endif

            return instance;
        }

        public virtual async ValueTask<TOut> Create(Transform? parent, CancellationToken cancellationToken)
        {
            var activeScene = SceneManagerHelper.ActiveScene;
            var instance = (await Object.InstantiateAsync(prefab, new InstantiateParameters { parent = parent.IfNull(defaultRoot) }, cancellationToken: cancellationToken))[0];

            if (!activeScene.IsValid()
#if UNITY_EDITOR
                ||
                !Application.isPlaying
#endif
                )
            {
                instance.DestroyByGameObject();
                throw new InvalidOperationException();
            }

#if ZENJECT_PLUGIN
            InjectInstance(instance);
#endif

            return instance;
        }

        public virtual async ValueTask<TOut> Create(InstantiateParameters prms, CancellationToken cancellationToken)
        {
            var activeScene = SceneManagerHelper.ActiveScene;
            var instance = (await Object.InstantiateAsync(prefab, prms, cancellationToken: cancellationToken))[0];

            if (!activeScene.IsValid()
#if UNITY_EDITOR
                ||
                !Application.isPlaying
#endif
                )
            {
                instance.DestroyByGameObject();
                throw new InvalidOperationException();
            }

#if ZENJECT_PLUGIN
            InjectInstance(instance);
#endif

            return instance;
        }

        public virtual async ValueTask<TOut[]> Create(int count, CancellationToken cancellationToken)
        {
            var activeScene = SceneManagerHelper.ActiveScene;
            var instances = await Object.InstantiateAsync(prefab, count, new InstantiateParameters { parent = default }, cancellationToken: cancellationToken);

            if (!activeScene.IsValid() 
#if UNITY_EDITOR
                ||
                !Application.isPlaying
#endif
                )
            {
                for (int i = 0; i < instances.Length; i++)
                    instances[i].DestroyByGameObject();

                throw new InvalidOperationException();
            }

#if ZENJECT_PLUGIN
            for (int i = 0; i < instances.Length; i++)
                InjectInstance(instances[i]);
#endif

            return instances;
        }

        public virtual async ValueTask<TOut[]> Create(int count, Transform? parent, CancellationToken cancellationToken)
        {
            var activeScene = SceneManagerHelper.ActiveScene;
            var instances = await Object.InstantiateAsync(prefab, count, new InstantiateParameters { parent = parent.IfNull(defaultRoot) }, cancellationToken: cancellationToken);

            if (!activeScene.IsValid()
#if UNITY_EDITOR
                ||
                !Application.isPlaying
#endif
                )
            {
                for (int i = 0; i < instances.Length; i++)
                    instances[i].DestroyByGameObject();

                throw new InvalidOperationException();
            }

#if ZENJECT_PLUGIN
            for (int i = 0; i < instances.Length; i++)
                InjectInstance(instances[i]);
#endif

            return instances;
        }
         
        public virtual async ValueTask<TOut[]> Create(int count, InstantiateParameters prms, CancellationToken cancellationToken)
        {
            var activeScene = SceneManagerHelper.ActiveScene;
            var instances = await Object.InstantiateAsync(prefab, count, prms, cancellationToken: cancellationToken);

            if (!activeScene.IsValid()
#if UNITY_EDITOR
                ||
                !Application.isPlaying
#endif
                )
            {
                for (int i = 0; i < instances.Length; i++)
                    instances[i].DestroyByGameObject();

                throw new InvalidOperationException();
            }

#if ZENJECT_PLUGIN
            for (int i = 0; i < instances.Length; i++)
                InjectInstance(instances[i]);
#endif

            return instances;
        }
    }
}
