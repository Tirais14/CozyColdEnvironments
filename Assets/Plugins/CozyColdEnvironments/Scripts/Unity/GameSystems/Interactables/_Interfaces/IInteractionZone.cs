using CCEnvs.Unity.GameSystems.Interactables;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IInteractionZone
    {
        Component InteractionAgent { get; }

        bool TryGetInteractable<T>(int? layers, [NotNullWhen(true)] out T? result)
            where T : IInteractable;

        bool TryGetInteractableWith<T>(int? layers, [NotNullWhen(true)] out T? result)
            where T : IInteractableWith;

        bool Contains(Type interactableType, int? layers);
        bool Contains(Vector2 point);
        bool Contains(Vector3 point);
        bool Contains(IInteractable? interactable);
        bool Contains(IInteractableWith? interactableWith);
    }
    public interface IInteractionZone<out T> : IInteractionZone
        where T : Component
    {
        new T InteractionAgent { get; }

        Component IInteractionZone.InteractionAgent => InteractionAgent;
    }
}
