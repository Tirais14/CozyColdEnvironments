using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using CCEnvs.Unity.UI.MVVM;
using ZLinq;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.GameSystems.Interactables
{
    /// <summary>
    /// The child should be call explicitly <see cref="TryAddInteractableBy(Component?)"/> and <see cref="TryRemoveInteractableBy(Component?)"/>
    /// </summary>
    public abstract class InteractionZone<TAgent> : CCBehaviour, IInteractionZone<TAgent>
        where TAgent : Component
    {
        protected readonly Dictionary<int, List<IInteractable>> interactables = new();
        protected readonly Dictionary<int, List<IInteractableWith>> interactableWiths = new();

        private int defaultLayer;
        private bool is2DCollider;

        [field: SerializeField, GetBySelf]
        public TAgent InteractionAgent { get; private set; } = null!;

        protected TAgent agent => InteractionAgent;

        protected override void Awake()
        {
            base.Awake();

            defaultLayer = LayerMask.NameToLayer("Default");
        }

        public bool Contains(Type interactableType, int? layerMask)
        {
            Guard.IsNotNull(interactableType, nameof(interactableType));

            if (interactableType.TrySwitchType(out bool result,
                (typeof(IInteractable), (type) =>
                {
                    if (interactables.TryGetValue(layerMask ?? defaultLayer, out var values))
                        return values.Exists(x => x.GetType().IsType(type));

                    return false;
                }
            ),
                (typeof(IInteractableWith), (type) =>
                {
                    if (interactableWiths.TryGetValue(layerMask ?? defaultLayer, out var values))
                        return values.Exists(x => x.GetType().IsType(type));

                    return false;
                }
            )))
                return result;

            return false;
        }
        public abstract bool Contains(Vector2 point);
        public abstract bool Contains(Vector3 point);
        public bool Contains(IInteractable? interactable)
        {
            if (interactable.IsNull())
                return false;

            return interactables.Values.ZL()
                                       .SelectMany(x => x)
                                       .Contains(interactable);
        }
        public bool Contains(IInteractableWith? interactableWith)
        {
            if (interactableWith.IsNull())
                return false;

            return interactableWiths.Values.ZL()
                                           .SelectMany(x => x)
                                           .Contains(interactableWith);
        }

        public bool TryGetInteractable<T>(int? layerMask, [NotNullWhen(true)] out T? result) where T : IInteractable
        {
            result = FindInteractable(typeof(T), layerMask).As<T>();

            return result.IsNotDefault();
        }

        public bool TryGetInteractableWith<T>(int? layerMask, [NotNullWhen(true)] out T? result) where T : IInteractableWith
        {
            result = FindInteractableWith(typeof(T), layerMask).As<T>();

            return result.IsNotDefault();
        }

        protected void TryAddInteractableBy(TAgent? agent)
        {
            if (agent == null)
                return;

            int layer = agent.gameObject.layer;

            if (agent.FindFor().Model<IInteractable>().TryAccess(out var interactable))
            {
                if (!interactables.TryGetValue(layer, out List<IInteractable> values))
                {
                    values = new List<IInteractable>();
                    interactables.Add(layer, values);
                }

                values.Add(interactable);
            }

            if (agent.FindFor().Model<IInteractableWith>().TryAccess(out var interactableWith))
            {
                if (!interactableWiths.TryGetValue(layer, out List<IInteractableWith> values))
                {
                    values = new List<IInteractableWith>();
                    interactableWiths.Add(layer, values);
                }

                values.Add(interactableWith);
            }
        }

        protected void TryRemoveInteractableBy(TAgent? agent)
        {
            if (agent == null)
                return;

            int layer = agent.gameObject.layer;

            if (agent.FindFor().Model<IInteractable>().TryAccess(out var interactable))
            {
                if (!interactables.TryGetValue(layer, out List<IInteractable> values))
                    return;

                values.Remove(interactable);
            }

            if (agent.FindFor().Model<IInteractableWith>().TryAccess(out var interactableWith))
            {
                if (!interactableWiths.TryGetValue(layer, out List<IInteractableWith> values))
                    return;

                values.Remove(interactableWith);
            }
        }

        private IInteractable? FindInteractable(Type type, int? layers)
        {
            if (interactables.TryGetValue(layers ?? defaultLayer, out List<IInteractable> values))
            {
                return values.ZL()
                             .OrderBy(x => x.InteractionPriority)
                             .FirstOrDefault(x => x.GetType().IsType(type));
            }

            return null;
        }

        private IInteractableWith? FindInteractableWith(Type type, int? layers)
        {
            if (interactableWiths.TryGetValue(layers ?? defaultLayer, out List<IInteractableWith> values))
            {
                return values.ZL()
                             .OrderBy(x => x.InteractionPriority)
                             .FirstOrDefault(x => x.GetType().IsType(type));
            }

            return null;
        }
    }
}
