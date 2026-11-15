using System;

#nullable enable
namespace CCEnvs.Services
{
    public interface IServiceInstaller
    {
        event Action<IServiceInstaller> OnInstalled;

        bool IsInstalled { get; }

        void AddBinding(IDisposable disposable);

        void AddBinder(IServiceBinder binder, BindingType bindingType);
    }

    public static class IServiceInstallerExtensions
    {
        public static void AddTo(this IDisposable disposable, IServiceInstaller installer)
        {
            CC.Guard.IsNotNull(disposable, nameof(disposable));
            CC.Guard.IsNotNull(installer, nameof(installer));

            installer.AddBinding(disposable);
        }
    }
}
