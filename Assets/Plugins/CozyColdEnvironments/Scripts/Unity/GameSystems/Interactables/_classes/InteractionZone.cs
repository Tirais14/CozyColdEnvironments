using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using ZLinq;

#nullable enable
#pragma warning disable S3236
namespace CCEnvs.Unity.GameSystems.Interactables
{
    /// <summary>
    /// The child should be call explicitly <see cref="TryAddInteractableBy(Component?)"/> and <see cref="TryRemoveInteractableBy(Component?)"/>
    /// </summary>
    public abstract class InteractionZone<TAgent> : CCBehaviour, IInteractionZone<TAgent>
        where TAgent : Component
    {
        private readonly Dictionary<int, List<IInteractable>> interactables = new();
        private readonly Dictionary<int, List<IInteractableWith>> interactableWiths = new();
        private readonly int defaultLayer = LayerMask.NameToLayer("Default");

        [GetBySelf]
        [field: SerializeField]
        public TAgent InteractionAgent { get; private set; } = null!;

        public bool Contains(Type interactableType, int? layers)
        {
            Guard.IsNotNull(interactableType, nameof(interactableType));

            if (interactableType.TrySwitchType(out bool result,
                (typeof(IInteractable), (type) =>
                {
                    if (interactables.TryGetValue(layers ?? defaultLayer, out var values))
                        return values.Exists(x => x.GetType().IsType(type));

                    return false;
                }
            ),
                (typeof(IInteractableWith), (type) =>
                {
                    if (interactableWiths.TryGetValue(layers ?? defaultLayer, out var values))
                        return values.Exists(x => x.GetType().IsType(type));

                    return false;
                }
            )))
                return result;

            return false;
        }

        public bool TryGetInteractable<T>(int? layers, [NotNullWhen(true)] out T? result) where T : IInteractable
        {
            result = FindInteractable(typeof(T), layers).As<T>();

            return result.IsNotDefault();
        }

        public bool TryGetInteractableWith<T>(int? layers, [NotNullWhen(true)] out T? result) where T : IInteractableWith
        {
            result = FindInteractableWith(typeof(T), layers).As<T>();

            return result.IsNotDefault();
        }

        protected void TryAddInteractableBy(Component? collider)
        {
            if (collider == null)
                return;

            int layer = collider.gameObject.layer;

            if (collider.TryGetAssignedObject<IInteractable>(out var interactable))
            {
                if (!interactables.TryGetValue(layer, out List<IInteractable> values))
                {
                    values = new List<IInteractable>();
                    interactables.Add(layer, values);
                }

                values.Add(interactable);
            }

            if (collider.TryGetAssignedObject<IInteractableWith>(out var interactableWith))
            {
                if (!interactableWiths.TryGetValue(layer, out List<IInteractableWith> values))
                {
                    values = new List<IInteractableWith>();
                    interactableWiths.Add(layer, values);
                }

                values.Add(interactableWith);
            }
        }

        protected void TryRemoveInteractableBy(Component? collider)
        {
            if (collider == null)
                return;

            int layer = collider.gameObject.layer;

            if (collider.TryGetAssignedObject<IInteractable>(out var interactable))
            {
                if (!interactables.TryGetValue(layer, out List<IInteractable> values))
                    return;

                values.Remove(interactable);
            }

            if (collider.TryGetAssignedObject<IInteractableWith>(out var interactableWith))
            {
                if (!interactableWiths.TryGetValue(layer, out List<IInteractableWith> values))
                    return;

                values.Remove(interactableWith);
            }
        }

        private IInteractable? FindInteractable(Type type, int? layers)
        {
            if (interactables.TryGetValue(layers ?? defaultLayer, out List<IInteractable> values))
                return values.ZL().OrderBy(x => x.Priority).FirstOrDefault(x => x.GetType().IsType(type));

            return null;
        }

        private IInteractableWith? FindInteractableWith(Type type, int? layers)
        {
            if (interactableWiths.TryGetValue(layers ?? defaultLayer, out List<IInteractableWith> values))
                return values.ZL().OrderBy(x => x.Priority).FirstOrDefault(x => x.GetType().IsType(type));

            return null;
        }
    }
}
