using CCEnvs.Patterns.Factories;
using System;
using System.Linq;

#nullable enable
namespace CCEnvs.Services
{
    public class FactoryBindingNode : IBindingNode
    {
        private readonly IFactory factory;

        private readonly BindingType bindingType;

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
                isCreated = true;
                return createdValue;
            }

            throw CC.ThrowHelper.InvalidOperationException(bindingType);
        }

        private object CreateInstance()
        {
            var method = ServiceBinderHelper.GetResolveMethod(factory.GetType(), nameof(IFactory<object>.Create));
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
