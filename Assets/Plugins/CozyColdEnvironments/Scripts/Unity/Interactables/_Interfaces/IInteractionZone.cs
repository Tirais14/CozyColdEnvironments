using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public interface IInteractionZone 
    {
        Type ItemType { get; }
        Type AgentType { get; }
        object InteractionAgent { get; }

        IEnumerable<object> GetInteractables();

        bool ContainsPoint(Vector2 point);
        bool ContainsPoint(Vector3 point);
        bool ContainsItem(object? item);
        bool ContainsAgent(object? agent);

        IObservable<object> ObserveItemEnter();

        IObservable<object> ObserveItemExit();
    }

    public interface IInteractionZone<TItem, TAgent> : IInteractionZone
    {
        new TAgent InteractionAgent { get; }

        object IInteractionZone.InteractionAgent => InteractionAgent!;

        new IEnumerable<TItem> GetInteractables();

        bool ContainsItem(TItem? item);
        bool ContainsAgent(TAgent? agent);

        new IObservable<TItem> ObserveItemEnter();

        new IObservable<TItem> ObserveItemExit();

        IEnumerable<object> IInteractionZone.GetInteractables()
        {
            return GetInteractables().Cast<object>();
        }

        bool IInteractionZone.ContainsItem(object? item)
        {
            return item is TItem && ContainsItem(item);
        }

        bool IInteractionZone.ContainsAgent(object? agent)
        {
            return agent is TAgent && ContainsAgent(agent);
        }

        IObservable<object> IInteractionZone.ObserveItemEnter()
        {
            return ObserveItemEnter().Cast<TItem, object>();
        }

        IObservable<object> IInteractionZone.ObserveItemExit()
        {
            return ObserveItemExit().Cast<TItem, object>();
        }
    }
}
