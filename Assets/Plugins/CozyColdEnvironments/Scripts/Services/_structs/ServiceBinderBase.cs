using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs.Services
{
    public class ServiceBinderBase
    {
        protected HashSet<Type>? contracts;

        private readonly Type baseContract;

        private readonly bool isAlreadyBinded;

        private Type[]? baseContractInterfaces;

        public object? ID { get; protected set; }
        public object? Instance { get; protected set; }

        public bool SkipBinded { get; protected set; }

        public IEnumerable<Type> Contracts {
            get
            {
                yield return baseContract;

                if (contracts is not null)
                    foreach (var contract in contracts)
                        yield return contract;
            }
        }

        public Type BaseContract => baseContract;

        public bool IsAlreadyBinded => isAlreadyBinded;

        public ServiceBinderBase(Type contract)
        {
            Guard.IsNotNull(contract, nameof(contract));
            baseContract = contract;
            isAlreadyBinded = CCServices.HasBinding(BaseContract, ID);   
        }

        public static ServiceBinderBase Create<TContract>()
        {
            return new ServiceBinderBase(typeof(TContract));
        }

        public LightDisposable<(IEnumerable<Type> Contracts, object? ID)> AsSingle()
        {
            return CCServices.Bind(this);
        }

        protected HashSet<Type> GetOrCreateContracts()
        {
            contracts ??= new HashSet<Type>();
            return contracts;
        }

        protected Type[] GetBaseContractInterfaces()
        {
            baseContractInterfaces ??= baseContract.GetInterfaces();
            return baseContractInterfaces;
        }
    }

    public class ServiceBinderBase<TSelf> : ServiceBinderBase
        where TSelf : ServiceBinderBase
    {
        public ServiceBinderBase(Type contract) : base(contract) { }

        public TSelf WithID(object? id)
        {
            ID = id;
            return this.CastTo<TSelf>();
        }

        public TSelf IfNotBound()
        {
            SkipBinded = true;
            return this.CastTo<TSelf>();
        }

        public TSelf WithInterfaces()
        {
            foreach (var iface in GetBaseContractInterfaces())
            {
                if (iface.Namespace.StartsWith("System") 
                    ||
                    (SkipBinded && CCServices.HasBinding(iface, ID)))
                {
                    continue;
                }

                GetOrCreateContracts().Add(iface);
            }

            return this.CastTo<TSelf>();
        }
        public TSelf WithInterfaces(string ifaceName, StringMatchSettings matchSettings = StringMatchSettings.Ordinal)
        {
            Guard.IsNotNullOrWhiteSpace(ifaceName);

            foreach (var iface in GetBaseContractInterfaces())
            {
                if (iface.Namespace.StartsWith("System")
                    ||
                    !iface.Name.Match(ifaceName, matchSettings))
                {
                    continue;
                }

                GetOrCreateContracts().Add(iface);
            }

            return this.CastTo<TSelf>();
        }

        public TSelf WithBaseTypes()
        {
            if (!BaseContract.IsValueType)
            {
                foreach (var baseType in BaseContract.CollectBaseTypes().Skip(1).SkipLast(1))
                {
                    if (SkipBinded && CCServices.HasBinding(baseType, ID))
                        continue;

                    GetOrCreateContracts().Add(baseType);
                }
            }

            return this.CastTo<TSelf>();
        }

        public TSelf FromInstance(object instance)
        {
            Instance = instance;
            return this.CastTo<TSelf>();
        }
    }
}
