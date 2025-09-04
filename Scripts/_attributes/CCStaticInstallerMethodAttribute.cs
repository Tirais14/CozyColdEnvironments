using System;

#nullable enable
namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CCStaticInstallerMethodAttribute : Attribute
    {
    }
}
