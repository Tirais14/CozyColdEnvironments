using CCEnvs.Disposables;
using CCEnvs.Services;
using CCEnvs.UnityX.Components;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Services
{
    [DefaultExecutionOrder(-9999)]
    public abstract class MonoServiceInstaller : CCBehaviour, IServiceInstaller
    {
        private readonly CCCompositeDisposable disposables = new();

        private bool isInstalled;

        private Action<IServiceInstaller>? onInstalled;

        public event Action<IServiceInstaller> OnInstalled {
            add
            {
                if (isInstalled)
                    return;

                onInstalled += value;
            }
            remove
            {
                if (onInstalled is null)
                    return;

                onInstalled -= value;
            }
        }

        protected sealed override void Awake()
        {
            base.Awake();
            InstallBindings();
            isInstalled = true;

            try
            {
                onInstalled?.Invoke(this);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
            finally
            {
                onInstalled = null;
            }

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

        protected abstract void InstallBindings();

        protected virtual void OnInstalledInternal() { }
    }
}
