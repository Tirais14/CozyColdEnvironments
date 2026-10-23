using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Services
{
    public sealed class BindingNode
    {
        public object? Target { get; private set; }

        public BindingType BindingType { get; }

        public bool IsResolved { get; private set; }
        public bool HasFactory { get; }

        public IFactory? Factory { get; }

        public BindingNode(
            object target,
            BindingType bindingType
            )
        {
            Guard.IsNotNull(target);

            Target = target;
            BindingType = bindingType;

            IsResolved = true;
        }

        public BindingNode(
            IFactory factory,
            BindingType bindingType
            )
        {
            Guard.IsNotNull(factory);

            Factory = factory;
            BindingType = bindingType;

            HasFactory = true;
        }

        public object GetInstance()
        {
            if (IsResolved)
                return Target!;

            if (BindingType == BindingType.Transient)
            {
                if (!HasFactory)
                    throw new InvalidOperationException("Invalid binding type");

                return Factory!.Create(this, Array.Empty<object>());
            }
            else if (BindingType == BindingType.Single)
            {
                if (!HasFactory)
                    throw new InvalidOperationException("Not found factory to resolve");

                if (!IsResolved)
                {
                    Target = Factory!.Create(this, Array.Empty<object>());
                    IsResolved = true;
                }

                return Target!;
            }

            throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Target), Target)
                .AddProperty(nameof(BindingType), BindingType)
                .ToStringAndDispose();
        }
    }
}
