using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UTIRLib.Attributes;
using UTIRLib.Injector;
using UTIRLib.InputSystem;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class UserInterface : MonoX, IUserInterface
    {
        [RequiredMember]
        [field: SerializeField]
        public EventSystem EventSystem { get; protected set; } = null!;

        [GetComponent]
        [field: SerializeField]
        public GraphicRaycaster DefaultRaycaster { get; protected set; } = null!;

        [RequiredMember]
        public IPointerHandler PointerHandler { get; protected set; } = null!;

        [RequiredMember]
        public IRaycasterUI Raycaster { get; protected set; } = null!;

        public bool IsOpened { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();

            Raycaster = new RaycasterUI(PointerHandler, DefaultRaycaster, EventSystem);
        }

        public virtual void Open()
        {
            IsOpened = true;
        }

        public virtual void Close()
        {
            IsOpened = false;
        }
    }
}