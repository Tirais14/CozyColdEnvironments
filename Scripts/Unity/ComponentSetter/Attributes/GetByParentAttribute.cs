using System;

#nullable enable
namespace CozyColdEnvironments
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponentInParent"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetByParentAttribute : GetComponentAttribute
    {
    
    }
}
