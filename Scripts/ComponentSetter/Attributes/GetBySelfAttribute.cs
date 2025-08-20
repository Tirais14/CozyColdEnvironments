using System;

#nullable enable
namespace UTIRLib
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponent"/>. Calls in <see cref="MonoX.Awake"/>
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
