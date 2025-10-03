#nullable enable
using System;
using UniRx;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public interface IWorldButton : IObservable<Unit>
    {
        event UnityAction OnClick;

        void Click();
    }
}
