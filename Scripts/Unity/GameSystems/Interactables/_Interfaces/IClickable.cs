#nullable enable
using CCEnvs.Returnables;
using System;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IClickable : IObservable<Mock>
    {
        event UnityAction OnClick;

        void Click();
    }
}
