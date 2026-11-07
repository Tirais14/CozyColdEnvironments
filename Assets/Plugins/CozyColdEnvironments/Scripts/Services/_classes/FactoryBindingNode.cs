using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Factories;
using CCEnvs.TypeMatching;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace CCEnvs.Services
{
    public sealed class FactoryBindingNode : IBindingNode, IDisposable
    {
        private readonly IFactory factory;

        private readonly BindingType bindingType;

        private readonly Lazy<List<object>> factored = new(() => new());

        private object? createdValue;

        private bool isCreated;

        private bool? isValueDisposable;
        private bool? isValueUtilizable;

        private int recursionDepth;

        public FactoryBindingNode(IFactory factory, BindingType bindingType)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
            this.bindingType = bindingType;
        }

        public object GetInstance()
        {
            if (recursionDepth >= 1)
            {
                //Type lastFactoryGenericArgument = factored.GetType().GetInterfaces()
                //    .Where(iface => iface.IsGenericType && iface.Name.StartsWith(nameof(IFactory)) && iface.Namespace.StartsWith(nameof(CCEnvs)))
                //    .Select(iface => iface.GetGenericArguments())
                //    .Select(args => args[args.Length - 1])
                //    .FirstOrDefault();

                throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Cannot resolve circular dependecy")
                    .AddProperty("Factory", factory.GetType())
                    .ToStringAndDispose()
                    );
            }

            recursionDepth++;

            try
            {
                if (isCreated)
                    return createdValue!;

                if (bindingType == BindingType.Transient)
                    return CreateInstance();
                else if (bindingType == BindingType.Single)
                {
                    createdValue = CreateInstance();

                    if (!isValueDisposable.HasValue ||
                        !isValueUtilizable.HasValue
                        )
                    {
                        isValueUtilizable = createdValue is IUtilizable;
                        isValueDisposable = createdValue is IDisposable;
                    }

                    if ((isValueDisposable.HasValue && isValueDisposable.Value) ||
                        (isValueUtilizable.HasValue && isValueUtilizable.Value)
                        )
                        factored.Value.Add(createdValue);

                    isCreated = true;
                    return createdValue;
                }

                throw CC.ThrowHelper.InvalidOperationException(bindingType);
            }
            finally
            {
                recursionDepth--;
            }
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (isCreated)
            {
                if (createdValue is IUtilizable createdValueUtilizable)
                    createdValueUtilizable.Utilize();
                else if (createdValue is IDisposable createdValueDisposable)
                    createdValueDisposable.Dispose();
            }

            if (factored.IsValueCreated)
            {
                if (isValueUtilizable.HasValue && isValueUtilizable.Value)
                    factored.Value.OfType<IUtilizable>().UtilizeEach(bufferized: false);
                else if (isValueDisposable.HasValue && isValueDisposable.Value)
                    factored.Value.OfType<IDisposable>().DisposeEach(bufferized: false);
            }

            if (factory is IUtilizable factoryUtilizable)
                factoryUtilizable.Utilize();
            else if (factory is IDisposable factoryDisposable)
                factoryDisposable.Dispose();
        }

        private object CreateInstance()
        {
            var method = ServiceBinderHelper.ResolveMethod(factory.GetType(), nameof(IFactory<object>.Create), cached: bindingType == BindingType.Transient);
            var arguments = ServiceBinderHelper.ResolveServices(method, bindingType == BindingType.Transient);
            return method.Invoke(factory, arguments.Is<object[]>(out var argumentArray) ? argumentArray : arguments.ToArray());
        }
    }
}
