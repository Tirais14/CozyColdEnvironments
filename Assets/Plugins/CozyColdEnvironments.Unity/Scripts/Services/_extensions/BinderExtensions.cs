using CCEnvs.Patterns.Factories;
using CCEnvs.Services;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.UnityX.Services
{
    public static class BinderExtensions
    {
        public static ServiceFactoryBinderNonGeneric FromComponentInHierarchy(
            this ServiceBinderNonGeneric binder,
            bool includeInactive = false
            )
        {
            Guard.IsNotNull(binder, nameof(binder));

            return new ServiceFactoryBinderNonGeneric(
                binder.Contract,
                Factory.Create((binder.Contract, includeInactive), (args) =>
                {
                    return GameObjectQuery.Scene.IncludeInactive(args.includeInactive).Component(args.Contract).Strict();
                }));
        }
        public static ServiceFactoryBinderGeneric<T> FromComponentInHierarchy<T>(
            this ServiceBinderGeneric<T> binder,
            bool includeInactive = false
            )
        {
            Guard.IsNotNull(binder, nameof(binder));

            return new ServiceFactoryBinderGeneric<T>(
                Factory.Create(includeInactive, includeInactive =>
                {
                    return GameObjectQuery.Scene.IncludeInactive(includeInactive).Component<T>().Strict();
                }));
        }

        public static ServiceFactoryBinderGeneric<TTo> FromComponentInHierarchy<TFrom, TTo>(
            this ServiceBinderGeneric<TFrom, TTo> binder,
            bool includeInactive = false
            )
            where TFrom : TTo
        {
            Guard.IsNotNull(binder, nameof(binder));

            return new ServiceFactoryBinderGeneric<TTo>(
                Factory.Create(includeInactive, includeInactive =>
                {
                    return (TTo)GameObjectQuery.Scene.IncludeInactive(includeInactive).Component<TFrom>().Strict();
                }));
        }
    }
}
