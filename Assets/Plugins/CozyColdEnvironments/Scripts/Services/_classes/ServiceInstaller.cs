#nullable enable
using CCEnvs.Disposables;
using System;
using System.Collections.Generic;

namespace CCEnvs.Services
{
    public abstract class ServiceInstaller : IServiceInstaller, IDisposable
    {
        private readonly CCCompositeDisposable disposables = new();

        private List<ResolvedBinder>? binders = new();

        private Action<IServiceInstaller>? onInstalled;

        public event Action<IServiceInstaller> OnInstalled {
            add
            {
                if (IsInstalled)
                    value.Invoke(this);
                else
                    onInstalled += value;
            }
            remove => onInstalled -= value;
        }

        public bool IsInstalled { get; private set; }

        public void Install()
        {
            try
            {
                InstallBindings();
                InstallBinders();
            }
            finally
            {
                IsInstalled = true;
            }

            onInstalled?.Invoke(this);
            OnInstalledInternal();
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

        public void Dispose()
        {
            disposables.Dispose();
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
        }
    }
}
