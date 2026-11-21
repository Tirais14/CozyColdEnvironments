using System;
using UnityEngine;
using VContainer;

#nullable enable
namespace CCEnvs.Unity.VContainer
{
    public class CustomComponentRegistrationBuilder : RegistrationBuilder
    {
        private bool includeInactive;

        private Transform? parent;

        public CustomComponentRegistrationBuilder(Type implementationType) : base(implementationType, Lifetime.Scoped)
        {
        }

        public override Registration Build()
        {
            var provider = new FindComponentProvider(ImplementationType, parent, includeInactive);
            return new Registration(ImplementationType, Lifetime.Scoped, InterfaceTypes, provider, Key);
        }

        public CustomComponentRegistrationBuilder IncludeInactive(bool state = true)
        {
            includeInactive = state;
            return this;
        }

        public CustomComponentRegistrationBuilder UnderTransform(Transform? parent)
        {
            this.parent = parent;
            return this;
        }
    }
}
