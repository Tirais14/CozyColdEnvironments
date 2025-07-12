using UnityEngine;
using UTIRLib.Utils;

#nullable enable
namespace UTIRLib
{
    public static class ComponentExtensions
    {
        public static T? AddComponent<T>(this Component component)
            where T : Component
        {
            return ComponentHelper.AddComponent<Component, T>(component);
        }
    }
}
