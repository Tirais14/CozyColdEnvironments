#nullable enable
namespace CCEnvs.Services
{
    public abstract class ServiceInstaller : IServiceInstaller
    {
        protected abstract void InstallBindings();
    }
}
