#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using System;
using UniRx;
using UnityEngine;

namespace CCEnvs.Unity.UI.Windows
{
    [DisallowMultipleComponent]
    public class Window : CCBehaviour, IWindow
    {
        public const string ALREADY_OPENED_MSG = "Already opened";

        private readonly Subject<Window> onOpen = new();
        private readonly Subject<Window> onClose = new();

        protected virtual bool OpenOnStart => false;

        public bool IsOpened { get; private set; }

        public IObservable<IWindow> OnOpen => onOpen.AsObservable();
        public IObservable<IWindow> OnClose => onClose.AsObservable();

        protected override void Start()
        {
            base.Start();

            if (OpenOnStart)
                Open();
            else
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
            onClose.OnNext(this);
            gameObject.SetActive(false);
        }

        public virtual void Open()
        {
            if (!CanOpen(out string message))
            {
                this.PrintWarning($"Cannot be opened. {message}");
                return;
            }

            gameObject.SetActive(true);
            onOpen.OnNext(this);
        }

        public bool SwitchOpenableState()
        {
            gameObject.SetActive(!IsOpened);

            return gameObject.activeSelf;
        }
    }
}
