using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using System;
using UniRx;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public class WorldButton : CCBehaviour, IWorldButton
    {
        private IObservable<Unit> observable = null!;

        private UnityAction? onClick;

        public event UnityAction OnClick {
            add => onClick += value;
            remove => onClick -= value;
        }

        protected override void OnAwake()
        {
            base.OnAwake();

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
