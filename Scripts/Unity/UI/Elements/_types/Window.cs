#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using System;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.UI.Elements
{
    [DisallowMultipleComponent]
    public class Window : CCBehaviour, IWindow
    {
        public const string ALREADY_OPENED_MSG = "Already opened";

        private readonly Subject<Window> onOpen = new();
        private readonly Subject<Window> onClose = new();

        public bool IsOpened { get; private set; }

        public IObservable<IWindow> OnOpen => onOpen.AsObservable();
        public IObservable<IWindow> OnClose => onClose.AsObservable();

        protected override void OnStart()
        {
            base.OnStart();

            Close();
        }

        public virtual bool CanOpen(out string message)
        {
            if (IsOpened)
            {
                message = ALREADY_OPENED_MSG;
                return false;
            }

            message = string.Empty;
            return true;
        }
        public bool CanOpen() => CanOpen(out _);

        public virtual void Close()
        {
            OnOpenInternal();
            onClose.OnNext(this);
            gameObject.SetActive(false);
        }

        public virtual void Open()
        {
            if (!CanOpen(out))
            {
                this.PrintWarning("Cannot be opened.");
                return;
            }

            gameObject.SetActive(true);
            OnCloseInternal();
            onOpen.OnNext(this);
        }

        public bool SwitchOpenableState()
        {
            gameObject.SetActive(!IsOpened);

            return gameObject.activeSelf;
        }

        protected virtual void OnOpenInternal()
        {
        }

        protected virtual void OnCloseInternal()
        {
        }
    }
}
