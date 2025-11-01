#nullable enable
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public interface IWorldButton : IInteractableVoid
    {
        event UnityAction OnClick;

        void Click();

        void IInteractableVoid.Interact() => Click();
    }
}
