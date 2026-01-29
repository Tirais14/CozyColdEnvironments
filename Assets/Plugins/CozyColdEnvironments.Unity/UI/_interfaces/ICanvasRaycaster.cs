using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface ICanvasRaycaster
    {
        GraphicRaycaster RaycasterGraphic { get; }

        object[] RaycastAll(Type type, Vector2 position, object? exclude = null);
        T[] RaycastAll<T>(Vector2 position, T? exclude = default);

        object? Raycast(Type type, Vector2 position, object? exclude = null);
        T? Raycast<T>(Vector2 position, T? exclude = default);

        bool TryRaycast(Type type,
                           Vector2 position,
                           [NotNullWhen(true)] out object? result,
                           object? exclude = null);
        bool TryRaycast<T>(Vector2 position,
                              [NotNullWhen(true)] out T? result,
                              T? exclude = default);
    }
}