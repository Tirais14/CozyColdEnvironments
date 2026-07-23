using System;

#nullable enable
namespace CCEnvs.UnityX.ComponentInjections
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponent"/>. Calls in <see cref="CCBehaviour.Awake"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field,
        Inherited = true,
        AllowMultiple = false
        )]
    public class GetBySelfAttribute : GetComponentAttribute
    {

    }
}
