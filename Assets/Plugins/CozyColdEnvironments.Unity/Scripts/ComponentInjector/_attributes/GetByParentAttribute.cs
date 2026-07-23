using System;

#nullable enable
namespace CCEnvs.UnityX.ComponentInjections
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponentInParent"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class GetByParentAttribute : GetComponentAttribute
    {
    }
}
