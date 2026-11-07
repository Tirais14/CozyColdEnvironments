using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;

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

        public static IDisposable AsSingle(this IServiceBinder binder)
        {
            CC.Guard.IsNotNull(binder, nameof(binder));
            return CCServices.Bind(binder, BindingType.Single);
        }

        public static IDisposable AsTransient(this IServiceBinder binder)
        {
            CC.Guard.IsNotNull(binder, nameof(binder));
            return CCServices.Bind(binder, BindingType.Transient);
        }

        public static TBinder Lazy<TBinder>(this TBinder binder)
            where TBinder : IServiceBindingLazyStateAccessor
        {
            binder.IsLazy = true;
            return binder;
        }

        public static TBinder NonLazy<TBinder>(this TBinder binder)
            where TBinder : IServiceBinder, IServiceBindingLazyStateAccessor
        {
            binder.IsLazy = false;
            return binder;
        }

        public static TBinder IfNotBound<TBinder>(this TBinder binder)
            where TBinder : IServiceBinder, IServiceBindingNotBoundStateAccessor
        {
            binder.IgnoreIfBound = true;
            return binder;
        }

        public static ServiceFactoryBinderNonGeneric FromResolve(this ServiceBinderNonGeneric binder, object? id = null)
        {
            Guard.IsNotNull(binder, nameof(binder));

            if (binder.ToContract is not null)
            {
                return new ServiceFactoryBinderNonGeneric(
                    binder.ToContract,
                    Factory.Create((binder.ToContract, id), args => CCServices.Resolve(args.ToContract, args.id))
                    );
            }
            else
            {
                return new ServiceFactoryBinderNonGeneric(
                    binder.Contract,
                    Factory.Create((binder.Contract, id), args => CCServices.Resolve(args.Contract, args.id))
                    );
            }
        }
        public static ServiceFactoryBinderGeneric<T> FromResolve<T>(this ServiceBinderGeneric<T> binder, object? id = null)
        {
            Guard.IsNotNull(binder, nameof(binder));
            return new ServiceFactoryBinderGeneric<T>(
                Factory.Create(id, id => CCServices.Resolve<T>(id))
                );
        }
        public static ServiceFactoryBinderGeneric<TTo> FromResolve<TFrom, TTo>(this ServiceBinderGeneric<TFrom, TTo> binder, object? id = null)
            where TFrom : TTo
        {
            Guard.IsNotNull(binder, nameof(binder));
            return new ServiceFactoryBinderGeneric<TTo>(
                Factory.Create(id, id => (TTo)CCServices.Resolve<TFrom>(id))
                );
        }
    }
}
