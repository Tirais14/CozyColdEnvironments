using System;

#nullable enable
namespace CCEnvs.UnityX.Injections
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponentInParent"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class GetByParentAttribute : GetComponentAttribute
    {
    }
}
