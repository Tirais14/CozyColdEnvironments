#nullable enable

using System;

namespace CCEnvs.Attributes
{
    /// <summary>
    /// Marks type as configuration installer by <see cref="CCProjectHelper"/>
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