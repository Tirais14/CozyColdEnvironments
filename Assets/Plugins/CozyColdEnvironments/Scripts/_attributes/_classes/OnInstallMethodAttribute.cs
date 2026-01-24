using System;

#nullable enable
namespace CCEnvs.Attributes
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Field,
        AllowMultiple = false, 
        Inherited = true
        )]
    public class OnInstallMethodAttribute : Attribute, ICCAttribute
    {
    }
}
