#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public interface IInteractableBase
    {
        int InteractionPriority => 0;
        bool IsInteractable => true;
        int InteractionLayer => -1;
    }
}
