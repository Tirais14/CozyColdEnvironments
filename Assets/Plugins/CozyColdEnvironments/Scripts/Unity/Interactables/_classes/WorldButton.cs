using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using System;
using UniRx;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public class WorldButton : CCBehaviour, IWorldButton
    {
        private IObservable<Unit> observable = null!;

        private UnityAction? onClick;

        public virtual int Priority { get; } = 0;
        public int LayerMask => gameObject.layer;

        public event UnityAction OnClick {
            add => onClick += value;
            remove => onClick -= value;
        }

        protected override void Awake()
        {
            base.Awake();

            observable = Observable.FromEvent<UnityAction>(
                (x) => () => x(),
                (x) => onClick += x,
                (x) => onClick -= x);
        }

        public virtual void Click()
        {
            onClick?.Invoke();
            observable.Publish();

            this.PrintLog("Clicked");
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return observable.Subscribe(observer);
        }
    }
}
