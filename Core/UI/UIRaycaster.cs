using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UTIRLib.Diagnostics;
using UTIRLib.InputSystem;
using UTIRLib.UExtensions;

#nullable enable

namespace UTIRLib.UI
{
    public class UIRaycaster : IRaycaster
    {
        protected readonly IPointerHandler pointerHandler;
        protected readonly PointerEventData eventData;
        protected readonly GraphicRaycaster graphicRaycaster;

        public UIRaycaster(IPointerHandler pointerHandler, GraphicRaycaster graphicRaycaster, EventSystem eventSystem)
        {
            this.pointerHandler = pointerHandler;
            this.graphicRaycaster = graphicRaycaster;
            eventData = new PointerEventData(eventSystem);
        }

        public bool TryRaycastFirst<T>(Vector2 position,
                                       [NotNullWhen(true)] out T? result,
                                       object? exclude = null) where T : class
        {
            result = RaycastFirst<T>(position, exclude);

            return result != null;
        }

        public bool TryRaycastFirst<T>([NotNullWhen(true)] out T? result, object? exclude = null)
             where T : class
        {
            TirLibDebug.Log("Using pointer handler position.", this, true);

            return TryRaycastFirst(pointerHandler.PointerPosition, out result, exclude);
        }

        public bool TryRaycastFirst(Type targetType,
                                    Vector2 position,
                                    [NotNullWhen(true)] out object? result,
                                    object? exclude = null)
        {
            result = RaycastFirst(targetType, position, exclude);

            return result != null;
        }

        public bool TryRaycastFirst(Type targetType,
                                    [NotNullWhen(true)] out object? result,
                                    object? exclude = null)
        {
            TirLibDebug.Log("Using pointer handler position.", this, true);

            return TryRaycastFirst(targetType, pointerHandler.PointerPosition, out result, exclude);
        }

        public bool TryRaycast<T>(Vector2 position, out T[] results, object? exclude = null)
        {
            results = Raycast(typeof(T), position, exclude) as T[] ?? Array.Empty<T>();

            return results.Length > 0;
        }

        public bool TryRaycast<T>(out T[] results, object? exclude = null)
        {
            TirLibDebug.Log("Using pointer handler position.", this, true);

            return TryRaycast(pointerHandler.PointerPosition, out results, exclude);
        }

        public bool TryRaycast(Type targetType, Vector2 position, out object[] results, object? exclude = null)
        {
            results = Raycast(targetType, position, exclude);

            return results.Length > 0;
        }

        public bool TryRaycast(Type targetType, out object[] results, object? exclude = null)
        {
            TirLibDebug.Log("Using pointer handler position.", this, true);

            return TryRaycast(targetType, pointerHandler.PointerPosition, out results, exclude);
        }

        public T? RaycastFirst<T>(Vector2 position, object? exclude = null)
            where T : class
        {
            return RaycastFirst(typeof(T), position, exclude) as T;
        }

        public T? RaycastFirst<T>(object? exclude = null)
            where T : class
        {
            TirLibDebug.Log("Using pointer handler position.", this, true);

            return RaycastFirst<T>(pointerHandler.PointerPosition, exclude);
        }

        public object? RaycastFirst(Type targetType, Vector2 position, object? exclude = null)
        {
            return RaycastInternal(targetType, position, exclude, onlyFirst: true);
        }

        public object? RaycastFirst(Type targetType, object? exclude = null)
        {
            TirLibDebug.Log("Using pointer handler position.", this, true);

            return RaycastFirst(targetType, pointerHandler.PointerPosition, exclude);
        }

        public T[] Raycast<T>(Vector2 position, object? exclude = null)
            where T : class
        {
            return Raycast<T>(position, exclude);
        }

        public T[] Raycast<T>(object? exclude = null)
            where T : class
        {
            TirLibDebug.Log("Using pointer handler position.", this, true);

            return Raycast(typeof(T), pointerHandler.PointerPosition, exclude).Cast<T>().ToArray();
        }

        public object[] Raycast(Type targetType, Vector2 position, object? exclude = null)
        {
            return (object[])(RaycastInternal(targetType, position, exclude, onlyFirst: false) ?? Array.Empty<object>());
        }

        public object[] Raycast(Type targetType, object? exclude = null)
        {
            TirLibDebug.Log("Using pointer handler position.", this, true);

            return Raycast(targetType, pointerHandler.PointerPosition, exclude);
        }

        protected RaycastResult[] GetRaycastResults(Vector2 position)
        {
            SetRaycastPosition(position);

            List<RaycastResult> results = new();
            graphicRaycaster.Raycast(eventData, results);

#if UNITY_EDITOR
            if (TirLibDebug.DrawEnabled)
            {
                GizmosDrawer.onDrawGizmos += () => Gizmos.DrawSphere(position, 1f);
            }
#endif

            return results.ToArray();
        }

        private static object? GetObjectFrom(Type targetType, RaycastResult raycastResult)
        {
            return raycastResult.gameObject.GetAssignedObject(targetType);
        }

        private static object? GetComponentFrom(Type targetType, RaycastResult raycastResult)
        {
            return raycastResult.gameObject.GetComponent(targetType);
        }

        private object? RaycastInternal(Type targetType, Vector2 position, object? exclude, bool onlyFirst = false)
        {
            RaycastResult[] raycastResults = GetRaycastResults(position);

            if (raycastResults.Length == 0) return Array.Empty<object>();

            return ProccessRaycastResults(targetType, raycastResults, exclude, onlyFirst);
        }

        private object? ProccessRaycastResults(Type targetType,
                                              RaycastResult[] raycastResults,
                                              object? exclude,
                                              bool onlyFirst)
        {
            bool targetTypeIsUnityComponent = typeof(Component).IsAssignableFrom(targetType);
            bool hasExclude = exclude.IsNotNull();

            int raycastResultsCount = raycastResults.Length;
            List<object>? results = onlyFirst ? null : new List<object>(raycastResultsCount);
            object? foundObject;
            for (int i = 0; i < raycastResultsCount; i++)
            {
                if (targetTypeIsUnityComponent)
                {
                    foundObject = GetComponentFrom(targetType, raycastResults[i]);

                    TirLibDebug.Log("Seeking object is unity component.", this, true);
                }
                else
                {
                    foundObject = GetObjectFrom(targetType, raycastResults[i]);

                    TirLibDebug.Log("Seeking object is other class.", this, true);
                }

                if (foundObject.IsNull()) continue;

                if (hasExclude && foundObject.Equals(exclude))
                {
                    TirLibDebug.Log($"Object {foundObject.GetTypeName()} skipped by exclude parameter.", this, true);

                    continue;
                }

                if (onlyFirst)
                {
                    TirLibDebug.Log("Raycast success.", this);

                    return foundObject;
                }
                else results!.Add(foundObject);
            }

            if (results != null)
            {
                TirLibDebug.Log("Raycast success.", this);
            }

            return results?.ToArray() ?? Array.Empty<object>();
        }

        private void SetRaycastPosition(Vector2 position)
        {
            eventData.position = position;

            TirLibDebug.Log($"Raycast position: {position}.", this, true);
        }
    }
}