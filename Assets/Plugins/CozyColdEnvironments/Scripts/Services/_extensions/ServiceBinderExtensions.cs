using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Services
{
    public static class ServiceBinderExtensions
    {
        public static void AsSingleFrom<TBinder>(this TBinder binder, IServiceInstaller installer)
            where TBinder : ISingleBindable, IServiceBinder
        {
            CC.Guard.IsNotNull(binder, nameof(binder));
            CC.Guard.IsNotNull(installer, nameof(installer));

            installer.OnInstalled += (installer) =>
            {
                installer.AddBinding(CCServices.Bind(binder, BindingType.Single));
            };
        }
        public static void AsSingleFrom<TBinder, TContract>(this TBinder binder, IServiceInstaller installer)
            where TBinder : ISingleBindable, IServiceBinder<TContract>
        {
            CC.Guard.IsNotNull(binder, nameof(binder));
            CC.Guard.IsNotNull(installer, nameof(installer));

            installer.OnInstalled += (installer) =>
            {
                installer.AddBinding(CCServices.Bind(binder, BindingType.Single));
            };
        }

        public static void AsTransientFrom<TBinder>(this TBinder binder, IServiceInstaller installer)
            where TBinder : ITransientBindable, IServiceBinder
        {
            CC.Guard.IsNotNull(binder, nameof(binder));
            CC.Guard.IsNotNull(installer, nameof(installer));

            installer.OnInstalled -= (installer) =>
            {
                installer.AddBinding(CCServices.Bind(binder, BindingType.Transient));
            };
        }
        public static void AsTransientFrom<TBinder, TContract>(this TBinder binder, IServiceInstaller installer)
            where TBinder : ITransientBindable, IServiceBinder<TContract>
        {
            CC.Guard.IsNotNull(binder, nameof(binder));
            CC.Guard.IsNotNull(installer, nameof(installer));

            installer.OnInstalled -= (installer) =>
            {
                installer.AddBinding(CCServices.Bind(binder, BindingType.Transient));
            };
        }
    }
}
