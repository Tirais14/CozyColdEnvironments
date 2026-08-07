using CCEnvs.TypeMatching;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX
{
    public static class GameObjectReference
    {
        public static bool TryResolve(object? target, [NotNullWhen(true)] out GameObject? result)
        {
            result = null;

            if (target.Is<IGameObjectReferenceContainer>(out var container))
                result = container.gameObject;
            else if (target.Is<Component>(out var component))
                result = component.gameObject;

            return result != null;
        }
    }
}
