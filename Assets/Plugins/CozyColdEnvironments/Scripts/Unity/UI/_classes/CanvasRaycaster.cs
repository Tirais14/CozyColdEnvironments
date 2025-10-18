#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Diagnostics;
using CCEnvs.Unity.Injections;
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

        public EventSystem EventSys { get; set; } = null!;

        [GetBySelf]
        public GraphicRaycaster RaycasterGraphic { get; private set; } = null!;

        protected override void Start()
        {
            base.Start();

            if (EventSys == null)
                EventSys = EventSystem.current;

            if (EventSys == null)
                throw new ObjectNotFoundException(typeof(EventSystem));

            pointerEventData = new PointerEventData(EventSys);
        }

        public object[] RaycastAll(Type type, Vector2 position, object? exclude = null)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            raycastResults.Clear();
            pointerEventData.position = position;
            RaycasterGraphic.Raycast(pointerEventData, raycastResults);

            return (from result in raycastResults
                    select result.gameObject.GetComponents(type) into cmps
                    where cmps.Length > 0
                    from cmp in cmps
                    select (object)cmp into cmp
                    where cmp != exclude
                    select cmp)
                    .ToArray();
        }
        public T[] RaycastAll<T>(Vector2 position, T? exclude = default)
        {
            return RaycastAll(typeof(T), position, exclude).Cast<T>().ToArray();
        }

        public object? Raycast(Type type, Vector2 position, object? exclude = null)
        {
            return RaycastAll(type, position, exclude).FirstOrDefault();
        }
        public T? Raycast<T>(Vector2 position, T? exclude = default)
        {
            return (T?)Raycast(typeof(T), position, exclude);
        }

        public bool TryRaycast(Type type,
                               Vector2 position,
                               [NotNullWhen(true)] out object? result,
                               object? exclude = null)
        {
            result = Raycast(type, position, exclude);

            return result.IsNotNull();
        }

        public bool TryRaycast<T>(Vector2 position,
                                  [NotNullWhen(true)] out T? result,
                                  T? exclude = default)
        {
            result = Raycast(position, exclude);

            return result.IsNotDefault();
        }
    }
}