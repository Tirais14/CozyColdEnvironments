using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Factories;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace CCEnvs.Services
{
    public static class CCServices
    {
        private static readonly Dictionary<BindingKey, IBindingNode> bindings = new();

        public static IDisposable Bind(IServiceBinder binder, BindingType bindingType)
        {
            CC.Guard.IsNotNull(binder, nameof(binder));

            object? id = null;

            if (binder.Is<IServiceBindingIDProvider>(out var idProvider))
                id = idProvider.ID;

            var bindingKey = new BindingKey(binder.Contract, id);
            bool ignoreIfBound = false;

            if (bindings.ContainsKey(bindingKey))
            {
                if (ignoreIfBound)
                    return CCDisposable.Empty;

                throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Binding already exists")
                    .AddProperty(nameof(binder.Contract), binder.Contract)
                    .AddProperty("ID", id)
                    .ToStringAndDispose()
                    );
            }

            if (binder.Is<IServiceBindingNotBoundStateProvider>(out var ifNotBoundProvider))
                ignoreIfBound = ifNotBoundProvider.IgnoreIfBound;

            if (binder is IServiceBindingFactoryProvider factoryProvider)
            {
                if (bindingType != BindingType.Transient &&
                    binder is IServiceBindingLazyStateProvider lazyStateProvider &&
                    !lazyStateProvider.IsLazy
                    )
                    bindings.Add(bindingKey, new InstanceBindingNode(factoryProvider.Factory.Create(Array.Empty<object>())));
                else
                    bindings.Add(bindingKey, new FactoryBindingNode(factoryProvider.Factory, bindingType));

            }
            else if (binder is IServiceBindingInstanceProvider instanceProvider)
            {
                if (bindingType == BindingType.Transient)
                {
                    throw new ArgumentException(DebugMessageBuilder.CreatePooled()
                        .AddMessage("Binder cannot be transient")
                        .AddProperty("Binder", binder.GetType())
                        .ToStringAndDispose()
                        );
                }

                bindings.Add(bindingKey, new InstanceBindingNode(instanceProvider.Instance));
            }
            else
            {
                throw new ArgumentException(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Invalid binder")
                    .AddProperty("Binder", binder.GetType())
                    .AddProperty("Providers", binder.GetType().GetInterfaces().Where(x => x.Name.EndsWith("provider", StringComparison.OrdinalIgnoreCase)).SequenceToString())
                    .ToStringAndDispose()
                    );
            }

            return CCDisposable.Create(bindingKey, bindingKey => Unbind(bindingKey));
        }

        public static ServiceBinderNonGeneric Bind(Type contract)
        {
            return new ServiceBinderNonGeneric(contract);
        }
        public static ServiceBinderGeneric<T> Bind<T>()
        {
            return new ServiceBinderGeneric<T>();
        }

        public static ServiceInstanceBinderNonGeneric BindInstance(object instance)
        {
            CC.Guard.IsNotNull(instance, nameof(instance));
            return new ServiceInstanceBinderNonGeneric(instance.GetType(), instance);
        }
        public static ServiceInstanceBinderGeneric<T> BindInstance<T>(T instance)
        {
            return new ServiceInstanceBinderGeneric<T>(instance);
        }

        public static ServiceFactoryBinderNonGeneric BindFactory(Type contract, IFactory factory)
        {
            return new ServiceFactoryBinderNonGeneric(contract, factory);
        }

        public static ServiceFactoryBinderGeneric<T> BindFactory<T>(IFactory<T> factory)
        {
            return new ServiceFactoryBinderGeneric<T>(factory);
        }

        public static object Resolve(Type contract, object? id = null)
        {
            Guard.IsNotNull(contract, nameof(contract));

            var bindingKey = new BindingKey(contract, id);

            if (!bindings.TryGetValue(bindingKey, out var result))
            {
                throw new CCException(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Not found binding")
                    .AddProperty(nameof(contract), contract)
                    .AddProperty(nameof(id), id)
                    .ToStringAndDispose()
                    );
            }

            return result.GetInstance();
        }
        public static T Resolve<T>(object? id = null)
        {
            return Resolve(typeof(T), id).CastTo<T>();
        }

        public static object? TryResolve(
            Type contract,
            object? id = null
            )
        {
            Guard.IsNotNull(contract, nameof(contract));

            if (!HasBinding(contract, id))
                return null;

            return Resolve(contract, id);
        }

        public static T? TryResolve<T>(object? id = null)
        {
            if (!HasBinding<T>(id))
                return default;

            return Resolve<T>(id)!;
        }

        public static bool TryResolveOut(
            Type contract,
            [NotNullWhen(true)] out object? result,
            object? id = null
            )
        {
            Guard.IsNotNull(contract);

            if (!HasBinding(contract, id))
            {
                result = null;
                return false;
            }

            result = Resolve(contract, id);
            return true;
        }

        public static bool TryResolveOut<TContract>(
            [NotNullWhen(true)] out TContract? result,
            object? id = null
            )
        {
            if (!HasBinding<TContract>(id))
            {
                result = default;
                return false;
            }

            result = Resolve<TContract>(id)!;
            return true;
        }

        public static IList<object> ResolveAll(Type contract)
        {
            Guard.IsNotNull(contract);

            var results = LightLazy.Create<IList<object>>(() => new List<object>());

            foreach (var binding in bindings)
                if (binding.Value.Is<object>(out var instance))
                    results.Value.Add(instance);

            return results.GetValue(Array.Empty<object>());
        }
        public static IList<TContract> ResolveAll<TContract>()
        {
            var results = LightLazy.Create<IList<TContract>>(() => new List<TContract>());

            foreach (var binding in bindings)
                if (binding.Value.Is<TContract>(out var instance))
                    results.Value.Add(instance);

            return results.GetValue(Array.Empty<TContract>());
        }

        public static bool Unbind(Type contractType, object? id = null)
        {
            if (!bindings.Remove(new BindingKey(contractType, id), out var node))
                return false;

            node.GetInstance().Maybe().Cast<IDisposable>().IfSome(x => x.Dispose());
            return true;
        }

        public static bool Unbind<TContract>(object? id = null)
        {
            return Unbind(typeof(TContract), id);
        }
        public static bool Unbind(BindingKey key)
        {
            return Unbind(key.Contract, key.ID);
        }

        public static bool HasBinding(Type type, object? id = null)
        {
            return bindings.ContainsKey(new BindingKey(type, id));
        }
        public static bool HasBinding<T>(object? id = null)
        {
            return HasBinding(typeof(T), id);
        }

        [OnInstallExecutable]
        private static void OnInstall()
        {
            bindings.Values.OfType<IDisposable>().DisposeEach();
            bindings.Clear();
        }
    }
}
