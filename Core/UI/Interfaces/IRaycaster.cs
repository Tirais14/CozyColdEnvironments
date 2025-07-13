using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable

namespace UTIRLib.UI
{
    public interface IRaycaster
    {
        public bool TryRaycastFirst<T>([NotNullWhen(true)] out T? result, object? exclude = null) where T : class;

        public bool TryRaycastFirst<T>(Vector2 position, [NotNullWhen(true)] out T? result, object? exclude = null)
            where T : class;

        public bool TryRaycastFirst(Type targetType, [NotNullWhen(true)] out object? result, object? exclude = null);

        public bool TryRaycastFirst(Type targetType, Vector2 position, [NotNullWhen(true)] out object? result,
            object? exclude = null);

        public bool TryRaycast<T>(out T[] results, object? exclude = null);

        public bool TryRaycast<T>(Vector2 position, out T[] results, object? exclude = null);

        public bool TryRaycast(Type targetType, out object[] results, object? exclude = null);

        public bool TryRaycast(Type targetType, Vector2 position, out object[] results, object? exclude = null);

        public T? RaycastFirst<T>(object? exclude = null) where T : class;

        public T? RaycastFirst<T>(Vector2 position, object? exclude = null) where T : class;

        public object? RaycastFirst(Type targetType, object? exclude = null);

        public object? RaycastFirst(Type targetType, Vector2 position, object? exclude = null);

        public T[] Raycast<T>(Vector2 position, object? exclude = null) where T : class;

        public T[] Raycast<T>(object? exclude = null) where T : class;

        public object[] Raycast(Type targetType, object? exclude = null);

        public object[] Raycast(Type targetType, Vector2 position, object? exclude = null);
    }
}