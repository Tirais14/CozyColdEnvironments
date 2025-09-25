#nullable enable
using System;
using UnityEngine;

namespace CCEnvs.Unity.UI.Elements
{
    [DisallowMultipleComponent]
    public class Window : CCBehaviour, IOpenable
    {
        public bool IsOpened { get; private set; }

        public IObservable<Window> OnOpen { get; private set; } = null!;
        public IObservable<Window> OnClose { get; private set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            OnOpen = Rx.Observable.Create(this);
            OnClose = Rx.Observable.Create(this);
        }

        public virtual void Close()
        {
            OnClose.As<Rx.Observable<Window>>().Publish();
            gameObject.SetActive(false);
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            OnOpen.As<Rx.Observable<Window>>().Publish();
        }

        public bool SwitchOpenableState()
        {
            gameObject.SetActive(!IsOpened);

            return gameObject.activeSelf;
        }
    }
}
