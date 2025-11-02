#nullable enable
using CCEnvs.FuncLanguage;
using System.Runtime.CompilerServices;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public interface IWorldButton : IInteractable
    {
        event UnityAction OnClick;

        void Click();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Either<InvokeInfo, Maybe<object>[]> IInteractable.Interact(params object?[]? tools)
        {
            return Either<InvokeInfo, Maybe<object>[]>.None;
        }
    }
}
