using System;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponentInChildren"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetByChildrenAttribute : GetComponentAttribute
    {
        public string? GameObejctName { get; }
        public bool HasGameObjectName => GameObejctName.IsNotNull();

        public GetByChildrenAttribute(string? gameObejctName = null)
        {
            GameObejctName = gameObejctName;
        }
    }
}
