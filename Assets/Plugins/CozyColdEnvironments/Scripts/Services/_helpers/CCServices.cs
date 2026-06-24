using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.FuncLanguage;
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
        private static readonly Dictionary<(Type Type, object? ID), object> bindings = new();

        public static LightDisposable<(IEnumerable<Type> Contracts, object? ID)> Bind(ServiceBinderBase serviceBinder)
        {
            if (serviceBinder.Instance.IsNull())
                return default;

            if (serviceBinder.IsAlreadyBinded && !serviceBinder.SkipBinded)
                throw new InvalidOperationException($"Already binded. Contract: {serviceBinder.BaseContract}; ID: {serviceBinder.ID.Maybe().Map(x => x.ToString()).GetValue("null")}");

            foreach (var contract in serviceBinder.Contracts)
            {
                if (HasBinding(contract, serviceBinder.ID))
                    continue;

                bindings.Add((contract, serviceBinder.ID), serviceBinder.Instance);
            }

            return CCDisposable.CreateLight((serviceBinder.Contracts, serviceBinder.ID),
                static (args) =>
                {
                    foreach (var contract in args.Contracts)
                        Unbind(contract, args.ID);
                });
        }

        public static ServiceBinder Bind(Type contract)
        {
            return new ServiceBinder(contract);
        }

        public static ServiceBinder<TContract> Bind<TContract>()
        {
            return new ServiceBinder<TContract>();
        }

        public static ServiceBinder<TContract> BindInstance<TContract>(TContract instance)
        {
            return new ServiceBinder<TContract>().FromInstance(instance);
        }

        public static object Resolve(Type contract, object? id = null)
        {
            Guard.IsNotNull(contract, nameof(contract));

            if (!bindings.TryGetValue((contract, id), out var result))
                throw new CCException($"Cannot find binding. Contract: {contract}; ID: {id.Maybe().Map(x => x.ToString()).GetValue("null")}");

            return result;
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

            return bindings.Remove((contractType, id));
        }

        public static bool Unbind<TContract>(object? id = null)
        {
            return Unbind(typeof(TContract), id);
        }

        public static bool HasBinding(Type type, object? id = null)
        {
            return bindings.ContainsKey((type, id));
        }
        public static bool HasBinding<T>(object? id = null)
        {
            return HasBinding(typeof(T), id);
        }

        [OnInstallExecutable]
        private static void OnInstall()
        {
            foreach (var binding in bindings.Values.OfType<IDisposable>())
                binding.Dispose();

            bindings.Clear();
        }
    }
}
