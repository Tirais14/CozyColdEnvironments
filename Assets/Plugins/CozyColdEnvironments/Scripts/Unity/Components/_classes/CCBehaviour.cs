using CCEnvs.Unity.Injections;
using Cysharp.Threading.Tasks;
using UnityEngine;

#nullable enable

#pragma warning disable IDE1006
namespace CCEnvs.Unity.Components
{
    public class CCBehaviour : MonoBehaviour
    {
        /// <summary>Cached</summary>
        public LazyCC<Transform> cTransform { get; private set; } = null!;
        /// <summary>Cached</summary>
        public LazyCC<GameObject> cGameObject { get; private set; } = null!;
        /// <summary>
        /// Is true before update and after start
        /// </summary>
        public bool StartPassed { get; private set; }

        protected virtual void Awake()
        {
            cTransform = new LazyCC<Transform>(() => transform);
            cGameObject = new LazyCC<GameObject>(() => gameObject);

            //Sets component fields and props marked by specical attribute
            ComponentInjector.Inject(this);
        }

        protected virtual void Start()
        {
            this.DoActionAsync(static async @this =>
            {
                await UniTask.Yield(PlayerLoopTiming.LastInitialization);

                @this.StartPassed = true;
                @this.PrintLog("Start Passed");
            });
        }

        //public void OnAfterStartAction<T>(T state, Action<T> action)
        //{
        //    Guard.IsNotNull(action);

        //    UniTask.Create((action, state, @this: this), static async input =>
        //    {
        //        await UniTask.WaitUntil(input.@this,
        //            static @this => @this.StartPassed,
        //            timing: PlayerLoopTiming.PreUpdate
        //            );

        //        input.action(input.state);
        //    })
        //    .AttachExternalCancellation(destroyCancellationToken)
        //    .SuppressCancellationThrow()
        //    .Forget();
        //}

        //public void OnAfterStartAction(Action action)
        //{
        //    Guard.IsNotNull(action);

        //    UniTask.Create((action, @this: this), static async input =>
        //    {
        //        await UniTask.WaitUntil(input.@this,
        //            static @this => @this.StartPassed,
        //            timing: PlayerLoopTiming.PreUpdate
        //            );

        //        input.action();
        //    })
        //    .AttachExternalCancellation(destroyCancellationToken)
        //    .SuppressCancellationThrow()
        //    .Forget();
        //}
    }
}