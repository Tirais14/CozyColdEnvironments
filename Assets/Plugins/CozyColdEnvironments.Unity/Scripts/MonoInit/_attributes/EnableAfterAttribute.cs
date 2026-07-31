using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.UnityX.MonoInit
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class EnableAfterAttribute : Attribute
    {
        public Type Type { get; }

        public EnableAfterAttribute(Type type)
        {
            Guard.IsNotNull(type);
            Type = type;
        }
    }
}
