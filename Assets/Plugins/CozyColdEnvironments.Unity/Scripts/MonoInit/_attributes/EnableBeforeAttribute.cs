using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.UnityX.MonoInit
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class EnableBeforeAttribute : Attribute
    {
        public Type Type { get; }

        public EnableBeforeAttribute(Type type)
        {
            Guard.IsNotNull(type);
            Type = type;
        }
    }
}
