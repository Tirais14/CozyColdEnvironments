using System;

#nullable enable
namespace CozyColdEnvironments
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponent"/>. Calls in <see cref="MonoCC.Awake"/>
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
