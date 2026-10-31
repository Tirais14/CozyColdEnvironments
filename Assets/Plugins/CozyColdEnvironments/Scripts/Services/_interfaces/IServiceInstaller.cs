using System;

#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceInstaller
    {
        event Action<IServiceInstaller> OnInstalled;

        void AddBinding(IDisposable disposable);
    }
}
