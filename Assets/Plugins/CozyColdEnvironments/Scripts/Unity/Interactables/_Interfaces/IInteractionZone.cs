using CCEnvs.Unity.Interactables;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IInteractionZone
    {
        Component InteractionAgent { get; }

        /// <returns>LINQ enumerator</returns>
        IEnumerable<IInteractableBase> GetInteractables();

        /// <inheritdoc cref="GetInteractables()"/>
        IEnumerable<T> GetInteractables<T>() where T : IInteractableBase;

        bool Contains(Vector2 point);
        bool Contains(Vector3 point);
        bool Contains(IInteractableBase? interactable);
    }
    public interface IInteractionZone<T> : IInteractionZone
        where T : Component
    {
        new T InteractionAgent { get; }

        Component IInteractionZone.InteractionAgent => InteractionAgent;

        bool Contains(T? agent);
    }
}
