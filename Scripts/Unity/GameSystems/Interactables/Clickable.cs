using CCEnvs.Diagnostics;
using CCEnvs.Returnables;
using CCEnvs.Rx;
using CCEnvs.Unity.Components;
using System;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public class Clickable : CCBehaviour, IClickable
    {
        private readonly Observable observable = new();

        private UnityAction? onClick;

        public event UnityAction OnClick {
            add => onClick += value;
            remove => onClick -= value;
        }

        public virtual void Click()
        {
            onClick?.Invoke();
            observable.Publish();

            CCDebug.PrintLog("Clicked", this);
        }

        public IDisposable Subscribe(IObserver<Mock> observer)
        {
            return observable.Subscribe(observer);
        }
    }
}
