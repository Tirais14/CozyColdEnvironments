using System;

#nullable enable
namespace UTIRLib
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponentInParent"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetByParentAttribute : ComponentContainableMemberAttribute
    {
    
    }
}
