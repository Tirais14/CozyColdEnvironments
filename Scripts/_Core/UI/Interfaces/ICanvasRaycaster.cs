using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace UTIRLib.UI
{
    public interface ICanvasRaycaster
    {
        GraphicRaycaster GraphicRaycaster { get; }

        object[] Raycast(Type type, Vector2 position, object? exclude = null);
        T[] Raycast<T>(Vector2 position, T? exclude = default);

        object? RaycastAny(Type type, Vector2 position, object? exclude = null);
        T? RaycastAny<T>(Vector2 position, T? exclude = default);

        bool TryRaycastAny(Type type,
                           Vector2 position,
                           out object? result,
                           object? exclude = null);
        bool TryRaycastAny<T>(Vector2 position,
                              out T? result,
                              T? exclude = default);
    }
}