using System;

#nullable enable
namespace CCEnvs.Attributes
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false, 
        Inherited = true
        )]
    public class OnInstallResetableAttribute : OnInstallAttribute, ICCAttribute
    {
    }
}
