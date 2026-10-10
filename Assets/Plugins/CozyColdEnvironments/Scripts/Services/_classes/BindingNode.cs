using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs.Services
{
    public class BindingNode
    {
        public object Target { get; }

        public BindingType BindingType { get; }

        public bool IsFactory { get; }

        public IReadOnlyList<BindingKey> ChildBindingKeys { get; }

        public BindingNode(
            object target,
            BindingType bindingType,
            bool isFactory,
            IEnumerable<BindingKey> childBindingKeys
            )
        {
            Guard.IsNotNull(target);
            CC.Guard.IsNotNull(childBindingKeys, nameof(childBindingKeys));

            Target = target;
            BindingType = bindingType;
            IsFactory = isFactory;
            ChildBindingKeys = childBindingKeys.ToArray();
        }

        public object GetInstance()
        {
            if (IsFactory)
                return Target.CastTo<IFactory<object>>().Create();

            return Target;
        }
    }
}
