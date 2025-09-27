using System;

#nullable enable
namespace CCEnvs.Unity.ComponentSetter
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponent"/>. Calls in <see cref="CCBehaviour.Awake"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field
                    |
                    AttributeTargets.Property,
        Inherited = true,
        AllowMultiple = false
        )]
    public class GetBySelfAttribute : GetComponentAttribute
    {

    }
}
