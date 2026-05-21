using System;

#nullable enable
namespace CCEnvs.UnityX.Injections
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponentInChildren"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class GetByChildrenAttribute : GetComponentAttribute
    {
    }
}
