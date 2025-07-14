using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UTIRLib.Attributes;
using UTIRLib.Diagnostics;
using UTIRLib.Injector;
using UTIRLib.InputSystem;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class UserInterface : MonoX, IUserInterface
    {
        protected readonly ReactiveProperty<bool> isOpenedProp = new();

        public event Action<IUserInterface>? OnStateChanged;

        [RequiredMember]
        [field: SerializeField]
        public EventSystem EventSystem { get; protected set; } = null!;

        [GetComponent]
        [field: SerializeField]
        public GraphicRaycaster DefaultRaycaster { get; protected set; } = null!;

        [RequiredMember]
        public IPointerHandler PointerHandler { get; protected set; } = null!;

        [RequiredMember]
        public IUIRaycaster Raycaster { get; protected set; } = null!;

        public bool IsOpened => isOpenedProp.Value;
        public IReadOnlyReactiveProperty<bool> IsOpenedProp => isOpenedProp;

        protected override void OnAwake()
        {
            base.OnAwake();

            Raycaster = new UIRaycaster(PointerHandler, DefaultRaycaster, EventSystem);
        }

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