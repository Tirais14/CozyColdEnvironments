using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.Interactables
{
    /// <summary>
    /// The child should be call explicitly <see cref="TryAddInteractableBy(Component?)"/> and <see cref="TryRemoveInteractableBy(Component?)"/>
    /// </summary>
    public abstract class InteractionZone<TAgent> : CCBehaviour, IInteractionZone<TAgent>
        where TAgent : Component
    {
        protected readonly C5.HashSet<TAgent> otherAgents = new();
        private readonly HashSet<IInteractable> interactables = new();

        [field: SerializeField, GetBySelf]
        public TAgent InteractionAgent { get; private set; } = null!;

        protected TAgent agent => InteractionAgent;

        protected override void Awake()
        {
            base.Awake();

            otherAgents.CollectionChanged += OnOtherAgentsChanged;
        }

        public IEnumerable<IInteractable> GetInteractables()
        {
            foreach (var item in from agent in otherAgents
                                 select agent.FindFor().Component<IInteractable>() into ible
                                 where ible.IsSome
                                 select ible.AccessUnsafe())
            {
                yield return item;
            }
        }

        public IEnumerable<T> GetInteractables<T>() where T : IInteractable
        {
            return GetInteractables().Where(x => x.Is<T>()).Cast<T>();
        }

        public abstract bool Contains(Vector2 point);
        public abstract bool Contains(Vector3 point);

        public bool Contains(IInteractable? interactable)
        {
            if (interactable.IsNull())
                return false;

            if (interactables.Count > 0)
                return interactables.Contains(interactable);

            interactables.AddRange(GetInteractables());

            return interactables.Contains(interactable);
        }

        public bool Contains(TAgent? agent)
        {
            if (agent == null)
                return false;

            return otherAgents.Contains(agent);
        }

        private void OnOtherAgentsChanged(object _)
        {
            if (interactables.Count > 0)
                interactables.Clear();
        }
    }
}
