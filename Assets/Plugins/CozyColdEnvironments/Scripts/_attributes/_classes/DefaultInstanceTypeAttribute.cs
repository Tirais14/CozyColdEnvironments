#nullable enable
using System;

namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Interface,
        AllowMultiple = false,
        Inherited = false)]
    public class DefaultInstanceTypeAttribute : Attribute, ICCAttribute
    {
        public Type InstanceType { get; }

        public DefaultInstanceTypeAttribute(Type instanceType)
        {
            this.InstanceType = instanceType;
        }
    }
}
