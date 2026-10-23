#nullable enable
using CCEnvs.Patterns.Factories;
using System;

namespace CCEnvs.Services
{
    public class ServiceBinderGeneric<T>
    {
        public ServiceBinderGeneric() { }

        public ServiceBinderGeneric<T, TTo> To<TTo>()
        {
            return new ServiceBinderGeneric<T, TTo>();
        }

        public ServiceInstanceBinderGeneric<T> FromInstance(T instance)
        {
            return new ServiceInstanceBinderGeneric<T>(instance);
        }

        public ServiceFactoryBinderGeneric<T> FromFactory(IFactory<T> factory)
        {
            return new ServiceFactoryBinderGeneric<T>(factory);
        }

        public ServiceFactoryBinderGeneric<T> FromMethod(Func<T> factory)
        {
            return new ServiceFactoryBinderGeneric<T>(Factory.Create(factory));
        }

        public ServiceFactoryBinderGeneric<T> FromNew()
        {
            return new ServiceFactoryBinderGeneric<T>(
                Factory.Create(() => ServiceBinderHelper.CreateByEmptyConstructor(typeof(T)).CastTo<T>())
                );
        }
        public ServiceFactoryBinderGeneric<T> FromNew<TNew>()
            where TNew : T, new()
        {
            return new ServiceFactoryBinderGeneric<T>(
                Factory.Create<T>(() => new TNew())
                );
        }
    }
    public class ServiceBinderGeneric<T, TTo>
    {
        public ServiceBinderGeneric() { }

        public ServiceInstanceBinderGeneric<TTo> FromInstance(TTo instance)
        {
            return new ServiceInstanceBinderGeneric<TTo>(instance);
        }

        public ServiceFactoryBinderGeneric<TTo> FromFactory(IFactory<TTo> factory)
        {
            return new ServiceFactoryBinderGeneric<TTo>(factory);
        }

        public ServiceFactoryBinderGeneric<TTo> FromMethod(Func<TTo> factory)
        {
            return new ServiceFactoryBinderGeneric<TTo>(Factory.Create(factory));
        }

        public ServiceFactoryBinderGeneric<TTo> FromNew()
        {
            return new ServiceFactoryBinderGeneric<TTo>(
                Factory.Create(() => ServiceBinderHelper.CreateByEmptyConstructor(typeof(T)).CastTo<TTo>())
                );
        }
        public ServiceFactoryBinderGeneric<TTo> FromNew<TNew>()
            where TNew : TTo, new()
        {
            return new ServiceFactoryBinderGeneric<TTo>(
                Factory.Create<TTo>(() => new TNew())
                );
        }
    }
}
