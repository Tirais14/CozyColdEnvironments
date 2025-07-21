using System;

#nullable enable
namespace UTIRLib
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponent"/>
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
