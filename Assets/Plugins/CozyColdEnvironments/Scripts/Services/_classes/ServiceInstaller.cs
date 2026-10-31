#nullable enable
using CCEnvs.Disposables;
using System;

namespace CCEnvs.Services
{
    public abstract class ServiceInstaller : IServiceInstaller, IDisposable
    {
        private readonly CCCompositeDisposable disposables = new();

        public event Action<IServiceInstaller>? OnInstalled;

        public void Install() => InstallBindings();

        public void AddBinding(IDisposable disposable)
        {
            CC.Guard.IsNotNull(disposable, nameof(disposable));
            disposables.Add(disposable);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        protected abstract void InstallBindings();
    }
}
