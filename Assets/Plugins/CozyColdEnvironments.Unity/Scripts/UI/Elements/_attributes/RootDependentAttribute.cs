using System;

#nullable enable
namespace CCEnvs.UnityX.UI
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RootDependentAttribute : Attribute
    {
    }
}
