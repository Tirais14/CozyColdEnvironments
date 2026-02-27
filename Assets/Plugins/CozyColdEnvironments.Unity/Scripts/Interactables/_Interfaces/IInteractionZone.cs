using System.Collections.Generic;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public interface IInteractionZone
    {
        object InteractionAgent { get; }
        IEnumerable<GameObject> GameObjects { get; }

        bool Contains(Vector2 point);
        bool Contains(Vector3 point);
        bool Contains(GameObject? gameObject);
        bool ContainsComponent(object? component);

        Observable<GameObject> ObserveOnEnter();

        Observable<GameObject> ObesrveOnStay();

        Observable<GameObject> ObserveOnExit();
    }

    public interface IInteractionZone<out TAgent> : IInteractionZone
    {
        new TAgent InteractionAgent { get; }

        object IInteractionZone.InteractionAgent => InteractionAgent!;
    }
}
