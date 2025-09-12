using CCEnvs.Attributes;
using System;

#nullable enable
namespace CCEnvs.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ResetOnEnterPlayModeAttribute : Attribute, ICCAttribute
    {
    }
}
