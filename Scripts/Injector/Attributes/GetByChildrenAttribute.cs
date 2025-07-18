using System;

#nullable enable
namespace UTIRLib
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponentInChildren"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetByChildrenAttribute : ComponentContainableMemberAttribute
    {
        
    }
}
