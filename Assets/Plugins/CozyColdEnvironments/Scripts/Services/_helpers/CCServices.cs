using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Factories;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using R3.Triggers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace CCEnvs.Services
{
    public static class CCServices
    {
        public readonly struct BindHandle
        {
            public IReadOnlyList<Type> Contracts { get; }

            public object? ID { get; }

            public BindHandle(IEnumerable<Type> contracts, object? iD)
            {
                CC.Guard.IsNotNull(contracts, nameof(contracts));

                Contracts = contracts.ToArray();
                ID = iD;
            }
        }

        private static readonly Dictionary<BindingKey, BindingNode> bindings = new();

        public static DisposableLight<BindHandle> Bind(
            IServiceBindingInfoProvider serviceBinder,
            BindingType bindingType
            )
        {
            if (serviceBinder.Target.IsNull())
                return default;

            var newBindings = new List<(BindingKey Key, BindingNode Node)>();

            foreach (var contract in serviceBinder.Contracts)
            {
                if (HasBinding(contract, serviceBinder.ID))
                {
                    if (!serviceBinder.IgnoreBinded)
                    {
                        throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                            .AddMessage("Binding already exists")
                            .AddProperty(nameof(contract), contract)
                            .AddProperty(nameof(serviceBinder.ID), serviceBinder.ID)
                            .ToStringAndDispose()
                            );
                    }

                    continue;
                }

                var bindingKey = new BindingKey(contract, serviceBinder.ID);

                var bindingNode = new BindingNode(
                    serviceBinder.Target,
                    bindingType,
                    serviceBinder.IsFactory,
                    Array.Empty<BindingKey>()
                    );

                newBindings.Add((bindingKey, bindingNode));
            }

            for (int i = 0; i < newBindings.Count; i++)
            {
                var (key, node) = newBindings[i];

                if (i == 0)
                {
                    node = new BindingNode(
                        node.Target,
                        node.BindingType,
                        node.IsFactory,
                        newBindings.Skip(1).Select(x => x.Key)
                        );
                }

                bindings.Add(key, node);
            }

            return CCDisposable.Light(new BindHandle(serviceBinder.Contracts, serviceBinder.ID),
                static (handle) =>
                {
                    foreach (var contract in handle.Contracts)
                        Unbind(contract, handle.ID);
                });
        }

        public static ServiceBinderBase Bind(Type contract)
        {
            return new ServiceBinderBase(contract);
        }

        public static ServiceBinderBase<TContract> Bind<TContract>()
        {
            return new ServiceBinderBase<TContract>();
        }

        public static InstanceServiceBinder BindInstance(object instance)
        {
            Guard.IsNotNull(instance, nameof(instance));
            return Bind(instance.GetType()).FromInstance(instance);
        }
        public static InstanceServiceBinder<TContract> BindInstance<TContract>(TContract instance)
        {
            return Bind<TContract>().FromInstance(instance);
        }

        public static FactoryServiceBinder BindFactory(Type contractType, IFactory<object> factory)
        {
            return Bind(contractType).AsFactory(factory);
        }
        public static FactoryServiceBinder<TContract> BindFactory<TContract>(IFactory<TContract> factory)
        {
            return Bind<TContract>().AsFactory(factory);
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
            Guard.IsNotNull(contractType, nameof(contractType));

            if (!bindings.Remove(new BindingKey(contractType, id), out BindingNode node))
                return false;

            foreach (var childBindingKey in node.ChildBindingKeys)
                Unbind(childBindingKey.Contract, childBindingKey.ID);

            return true;
        }

        public static bool Unbind<TContract>(object? id = null)
        {
            return Unbind(typeof(TContract), id);
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
