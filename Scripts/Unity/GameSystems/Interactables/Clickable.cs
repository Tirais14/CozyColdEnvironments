using CCEnvs.Observables;
using CCEnvs.Returnables;
using System;
using UnityEngine;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public class Clickable : Component, IClickable
    {
        private readonly Observable observable = new();

        [SerializeField]
        private UnityEvent onClick = new();

        public event UnityAction OnClick {
            add => onClick.AddListener(value);
            remove => onClick.RemoveListener(value);
        }

        public virtual void Click() => onClick.Invoke();

        public IDisposable Subscribe(IObserver<Mock> observer)
        {
            return observable.Subscribe(observer);
        }
    }
}
