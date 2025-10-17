using CCEnvs.Unity.GameSystems.Interactables;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public interface IInteractionZone
    {
        float Size { get; }

        bool TryGetInteractable<T>(int? layers, [NotNullWhen(true)] out T? result)
            where T : IInteractable;

        bool TryGetInteractableWith<T>(int? layers, [NotNullWhen(true)] out T? result)
            where T : IInteractableWith;

        bool Contains(Type interactableType, int? layers);

        void SetSize(float size);
    }
}
