#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UTIRLib.Diagnostics;
using UTIRLib.Unity.Extensions;

namespace UTIRLib.UI
{
    public sealed class CanvasRaycaster : ICanvasRaycaster
    {
        private readonly PointerEventData pointerEventData;
        private readonly List<RaycastResult> raycastResults = new();

        public GraphicRaycaster GraphicRaycaster { get; }

        public CanvasRaycaster(EventSystem eventSystem, GraphicRaycaster graphicRaycaster)
        {
            pointerEventData = new PointerEventData(eventSystem);
            GraphicRaycaster = graphicRaycaster;
        }

        public object[] Raycast(Type type, Vector2 position, object? exclude = null)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            pointerEventData.position = position;
            GraphicRaycaster.Raycast(pointerEventData, raycastResults);

            var results =new List<object>();
            for (int i = 0; i < raycastResults.Count; i++)
                results.AddRange(raycastResults[i].gameObject.GetAssignedObjects(type));

            if (exclude.IsNotNull())
                results.Remove(exclude);

            return results.ToArray();
        }
        public T[] Raycast<T>(Vector2 position, T? exclude = default)
        {
            return Raycast(typeof(T), position, exclude).Cast<T>().ToArray();
        }

        public object? RaycastAny(Type type, Vector2 position, object? exclude = null)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            pointerEventData.position = position;
            GraphicRaycaster.Raycast(pointerEventData, raycastResults);

            for (int i = 0; i < raycastResults.Count; i++)
            {
                if (raycastResults[i].gameObject.TryGetAssignedObject(type, out object? result)
                    &&
                    result != exclude
                    )
                    return result;
            }

            return null;
        }
        public T? RaycastAny<T>(Vector2 position, T? exclude = default)
        {
            return (T?)RaycastAny(typeof(T), position, exclude);
        }

        public bool TryRaycastAny(Type type,
                                  Vector2 position,
                                  out object? result,
                                  object? exclude = null)
        {
            result = RaycastAny(type, position, exclude);

            return result.IsNotNull();
        }

        public bool TryRaycastAny<T>(Vector2 position,
                                     out T? result,
                                     T? exclude = default)
        {
            result = RaycastAny(position, exclude);

            return result.IsNotDefault();
        }
    }
}