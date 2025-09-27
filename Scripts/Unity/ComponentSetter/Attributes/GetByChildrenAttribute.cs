using CCEnvs.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Unity.ComponentSetter
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
