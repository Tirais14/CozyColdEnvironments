#nullable enable

using System;

namespace CCEnvs.Attributes
{
    /// <summary>
    /// Marks type as configuration installer by <see cref="Install"/>
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class
        |
        AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true)]
    public class StaticInstallerAttribute : Attribute
    {
    }
}