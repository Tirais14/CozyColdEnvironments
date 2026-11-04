using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Factories;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public FactoryBindingNode(IFactory factory, BindingType bindingType)
        {
            CC.Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
            this.bindingType = bindingType;
        }

        public object GetInstance()
        {
            if (isCreated)
                return createdValue!;

            if (bindingType == BindingType.Transient)
                return CreateInstance();
            else if (bindingType == BindingType.Single)
            {
                createdValue = CreateInstance();
                factored.Value.Add(createdValue);
                isCreated = true;
                return createdValue;
            }

            throw CC.ThrowHelper.InvalidOperationException(bindingType);
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (isCreated)
                createdValue.Maybe().Cast<IDisposable>().IfSome(x => x.Dispose());

            if (factored.IsValueCreated)
                factored.Value.OfType<IDisposable>().DisposeEach(bufferized: false);

            factory.Maybe().Cast<IDisposable>().IfSome(x => x.Dispose());
        }

        private object CreateInstance()
        {
            var method = ServiceBinderHelper.ResolveMethod(factory.GetType(), nameof(IFactory<object>.Create), cached: bindingType == BindingType.Transient);
            var arguments = ServiceBinderHelper.ResolveServices(method);
            return method.Invoke(factory, arguments.ToArray());
        }
    }

    //public class FactoryBindingNode<T> : IBindingNode
    //{
    //    private readonly IFactory<T> factory;

    //    private readonly BindingType bindingType;

    //    private object? createdValue;

    //    private bool isCreated;

    //    public FactoryBindingNode(IFactory<T> factory, BindingType bindingType)
    //    {
    //        CC.Guard.IsNotNull(factory, nameof(factory));

    //        this.factory = factory;
    //        this.bindingType = bindingType;
    //    }

    //    public object GetInstance()
    //    {
    //        if (isCreated)
    //            return createdValue!;

    //        if (bindingType == BindingType.Transient)
    //            return factory.Create()!;

    //        else if (bindingType == BindingType.Single)
    //        {
    //            createdValue = factory.Create();
    //            isCreated = true;
    //            return createdValue!;
    //        }

    //        throw CC.ThrowHelper.InvalidOperationException(bindingType);
    //    }
    //}
}
