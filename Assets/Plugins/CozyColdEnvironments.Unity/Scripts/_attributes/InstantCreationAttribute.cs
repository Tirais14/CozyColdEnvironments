using CCEnvs.Attributes;
using System;

#nullable enable
namespace CCEnvs.UnityX.Attributes
{
    /// <summary>
    /// Marks <see cref="CCBehaviourStatic"/> to create the instance before the first call
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InstantCreationAttribute : Attribute, ICCAttribute
    {
    }
}
