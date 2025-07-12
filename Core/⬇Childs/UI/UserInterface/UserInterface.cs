using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UTIRLib.Injector;
using UTIRLib.InputSystem;

#nullable enable

namespace UTIRLib.UI
{
    public class UserInterface : MonoX, IUserInterface
    {
        protected readonly ReactiveProperty<bool> isOpenedProp = new();

        public event Action<IUserInterface>? OnStateChanged;

        [field: SerializeField]
        public EventSystem EventSystem { get; protected set; } = null!;

        [field: SerializeField]
        [GetComponentInParentIfNull]
        public GraphicRaycaster DefaultRaycaster { get; protected set; } = null!;

        public IPointerHandler PointerHandler { get; protected set; } = null!;
        public IRaycaster Raycaster { get; protected set; } = null!;
        public bool IsOpened => isOpenedProp.Value;
        public IReadOnlyReactiveProperty<bool> IsOpenedProp => isOpenedProp;

        public virtual void Open()
        {
            isOpenedProp.Value = true;
            OnStateChanged?.Invoke(this);
        }

        public virtual void Close()
        {
            isOpenedProp.Value = false;
            OnStateChanged?.Invoke(this);
        }
    }
}