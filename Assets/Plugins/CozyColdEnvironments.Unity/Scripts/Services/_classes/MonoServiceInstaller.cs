using CCEnvs.Disposables;
using CCEnvs.Services;
using CCEnvs.UnityX.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Services
{
    [DefaultExecutionOrder(-9999)]
    public abstract class MonoServiceInstaller : CCBehaviour, IServiceInstaller
    {
        private readonly CCCompositeDisposable disposables = new();

        private List<ResolvedBinder>? binders = new();

        private bool isInstalled;

        private Action<IServiceInstaller>? onInstalled;

        public event Action<IServiceInstaller> OnInstalled {
            add
            {
                if (isInstalled)
                    value.Invoke(this);
                else
                    onInstalled += value;
            }
            remove => onInstalled -= value;
        }

        public bool IsInstalled => isInstalled;

        protected sealed override void Awake()
        {
            base.Awake();

            try
            {
                InstallBindings();
                InstallBinders();
            }
            finally
            {
                isInstalled = true;
            }

            onInstalled?.Invoke(this);
            OnInstalledInternal();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            disposables.Dispose();
        }

        public void AddBinding(IDisposable disposable)
        {
            CC.Guard.IsNotNull(disposable, nameof(disposable));
            disposables.Add(disposable);
        }

        public void AddBinder(IServiceBinder binder, BindingType bindingType)
        {
            CC.Guard.IsNotNull(binder, nameof(binder));

            if (IsInstalled)
                return;

            binders?.Add(new ResolvedBinder(binder, bindingType));
        }

        protected abstract void InstallBindings();

        protected virtual void OnInstalledInternal() { }

        private void InstallBinders()
        {
            if (binders is null)
                return;

            for (int i = 0; i < binders.Count; i++)
            {
                var binder = binders[i];
                CCServices.Bind(binder.Value, binder.Type)
                    .AddTo(this);
            }

            binders = null;
        }
    }
}
