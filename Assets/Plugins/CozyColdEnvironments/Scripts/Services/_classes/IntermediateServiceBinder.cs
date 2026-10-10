using CCEnvs.Disposables;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs.Services
{
    public class IntermediateServiceBinder<TSelf>
    {
        protected readonly HashSet<Type> contracts = new();

        private Type[]? baseContractInterfaces;

        public object? ID { get; protected set; }

        public bool IgnoreBinded { get; protected set; }

        public IEnumerable<Type> Contracts {
            get => contracts.Prepend(BinderBase.BaseContract);
        }

        public ServiceBinderBase BinderBase { get; }

        public IntermediateServiceBinder(ServiceBinderBase binderBase)
        {
            Guard.IsNotNull(binderBase);
            BinderBase = binderBase;
        }

        public DisposableLight<CCServices.BindHandle> AsSingle()
        {
            return CCServices.Bind(this.CastTo<IServiceBindingInfoProvider>(), BindingType.Single);
        }

        public TSelf IfNotBound()
        {
            IgnoreBinded = true;
            return this.CastTo<TSelf>();
        }

        public TSelf WithID(object? id)
        {
            ID = id;
            return this.CastTo<TSelf>();
        }

        public TSelf WithInterfaces()
        {
            foreach (var iface in GetBaseContractInterfaces())
            {
                if (iface.Namespace.StartsWith("System"))
                    continue;

                contracts.Add(iface);
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

                contracts.Add(iface);
            }

            return this.CastTo<TSelf>();
        }

        public TSelf WithBaseTypes()
        {
            if (!BinderBase.BaseContract.IsValueType)
                foreach (var baseType in BinderBase.BaseContract.CollectBaseTypes().Skip(1).SkipLast(1))
                    contracts.Add(baseType);

            return this.CastTo<TSelf>();
        }

        protected Type[] GetBaseContractInterfaces()
        {
            baseContractInterfaces ??= BinderBase.BaseContract.GetInterfaces();
            return baseContractInterfaces;
        }
    }
}
