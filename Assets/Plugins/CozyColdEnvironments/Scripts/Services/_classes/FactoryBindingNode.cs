using CCEnvs.Patterns.Factories;
using System;

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
            {
                return factory.Create(this, Array.Empty<object>());
            }
            else if (bindingType == BindingType.Single)
            {
                createdValue = factory.Create(Array.Empty<object>());
                isCreated = true;
                return createdValue;
            }

            throw CC.ThrowHelper.InvalidOperationException(bindingType);
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
