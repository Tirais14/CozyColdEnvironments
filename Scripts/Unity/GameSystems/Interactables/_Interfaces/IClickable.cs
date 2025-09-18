#nullable enable
using System;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IClickable : IObservable<Returnables.Mock>
    {
        event UnityAction OnClick;

        void Click();
    }
}
