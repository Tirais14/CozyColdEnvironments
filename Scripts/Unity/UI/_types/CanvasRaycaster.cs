#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Diagnostics;
using CCEnvs.Unity.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CCEnvs.Unity.UI
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public sealed class CanvasRaycaster : CCBehaviour, ICanvasRaycaster
    {
        private readonly List<RaycastResult> raycastResults = new();
        private PointerEventData pointerEventData = null!;

        [Tooltip("Keep null to use current")]
        public EventSystem EventSys { get; private set; } = null!;
        [GetBySelf]
        public GraphicRaycaster RaycasterGraphic { get; private set; } = null!;

        protected override void OnStart()
        {
            base.OnStart();

            if (EventSys == null)
                EventSys = EventSystem.current;

            if (EventSys == null)
                throw new ObjectNotFoundException(typeof(EventSystem));

            pointerEventData = new PointerEventData(EventSys);
        }

        public object[] Raycast(Type type, Vector2 position, object? exclude = null)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            raycastResults.Clear();
            pointerEventData.position = position;
            RaycasterGraphic.Raycast(pointerEventData, raycastResults);

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

            raycastResults.Clear();
            pointerEventData.position = position;
            RaycasterGraphic.Raycast(pointerEventData, raycastResults);

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
                                  [NotNullWhen(true)] out object? result,
                                  object? exclude = null)
        {
            result = RaycastAny(type, position, exclude);

            return result.IsNotNull();
        }

        public bool TryRaycastAny<T>(Vector2 position,
                                     [NotNullWhen(true)] out T? result,
                                     T? exclude = default)
        {
            result = RaycastAny(position, exclude);

            return result.IsNotDefault();
        }
    }
}