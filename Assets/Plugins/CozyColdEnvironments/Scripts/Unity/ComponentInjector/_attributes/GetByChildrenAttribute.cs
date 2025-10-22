using CCEnvs.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Unity.Injections
{
    /// <summary>
    /// Same as <see cref="UnityEngine.Component.GetComponentInChildren"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
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
