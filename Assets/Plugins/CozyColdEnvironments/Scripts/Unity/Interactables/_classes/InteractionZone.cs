using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public abstract class InteractionZone<TItem, TAgent> : CCBehaviour, IInteractionZone<TItem, TAgent>
        where TAgent : Component
    {
        protected readonly C5.HashSet<TAgent> otherAgents = new();
        protected readonly HashSet<TItem> items = new();
        protected Subject<TItem>? itemEnterSubj;
        protected Subject<TItem>? itemExitSubj;

        [field: SerializeField, GetBySelf]
        public TAgent InteractionAgent { get; private set; } = null!;

        public Type ItemType { get; } = typeof(TItem);
        public Type AgentType { get; } = typeof(TAgent);

        protected TAgent agent => InteractionAgent;

        protected override void Awake()
        {
            base.Awake();

            otherAgents.CollectionChanged += OnOtherAgentsChanged;
        }

        public IEnumerable<TItem> GetInteractables()
        {
            foreach (var item in from agent in otherAgents.ZL()
                                 select agent.AppealTo().Component<TItem>().Lax() into item
                                 where item.IsSome
                                 select item.GetValueUnsafe())
            {
                yield return item;
            }
        }

        public abstract bool ContainsPoint(Vector2 point);
        public abstract bool ContainsPoint(Vector3 point);

        public bool ContainsItem(TItem? item)
        {
            if (item.IsNull())
                return false;

            if (items.Count > 0)
                return items.Contains(item);

            items.AddRange(GetInteractables());

            return items.Contains(item);
        }

        public bool ContainsAgent(TAgent? agent)
        {
            if (agent == null)
                return false;

            return otherAgents.Contains(agent);
        }

        public IObservable<TItem> ObserveItemEnter()
        {
            if (itemEnterSubj is null)
            {
                itemEnterSubj = new Subject<TItem>();
                itemEnterSubj.AddTo(this);
            }
            
            return itemEnterSubj;
        }

        public IObservable<TItem> ObserveItemExit()
        {
            if (itemExitSubj is null)
            {
                itemExitSubj = new Subject<TItem>();
                itemExitSubj.AddTo(this);
            }

            return itemExitSubj;
        }

        private void OnOtherAgentsChanged(object _)
        {
            if (items.Count > 0)
                items.Clear();
        }
    }
    public class InteractionZone : InteractionZone<object, Collider>
    {
        protected override void Start()
        {
            base.Start();
            agent.isTrigger = true;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (items.Add(other) && itemEnterSubj is not null)
                itemEnterSubj.OnNext(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (items.Remove(other) && itemExitSubj is not null)
                itemExitSubj.OnNext(other);
        }

        public override bool ContainsPoint(Vector3 point)
        {
            return agent.bounds.Contains(point);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return ContainsPoint((Vector3)point);
        }
    }

    public class InteractionZone2D : InteractionZone<object, Collider2D>
    {
        protected override void Start()
        {
            base.Start();
            agent.isTrigger = true;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (items.Add(other) && itemEnterSubj is not null)
                itemEnterSubj.OnNext(other);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (items.Remove(other) && itemExitSubj is not null)
                itemExitSubj.OnNext(other);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return agent.OverlapPoint(point);
        }

        public override bool ContainsPoint(Vector3 point)
        {
            return ContainsPoint((Vector2)point);
        }
    }
}
