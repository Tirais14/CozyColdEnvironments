#nullable enable
using System;

namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, 
        AllowMultiple = true, 
        Inherited = true)]
    public class ConstructorOfTypeAttribute : Attribute, ICCAttribute
    {
        public Type ConstructableType { get; }

        public ConstructorOfTypeAttribute(Type constructableType)
        {
            ConstructableType = constructableType;
        }
    }
}
