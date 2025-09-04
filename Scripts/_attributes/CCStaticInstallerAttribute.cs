#nullable enable

using System;

namespace CCEnvs.Attributes
{
    /// <summary>
    /// Marks type as configuration installer by CCInstaller
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class
        |
        AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true)]
    public class CCStaticInstallerAttribute : Attribute
    {
    }
}