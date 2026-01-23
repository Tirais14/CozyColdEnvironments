using System;

#nullable enable
namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnInstallMethodAttribute : Attribute, ICCAttribute
    {
    }
}
