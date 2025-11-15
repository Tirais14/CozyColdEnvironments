using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Interactables;
using UnityEngine;
using UnityEngine.Events;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public class Interactable : IInteractable
    {
        [field: SerializeField]
        public UnityEvent OnInteract { get; protected set; } = null!;

        public bool RequireInput => false;
        public bool HasOutput => false;

        public bool TryInteract(object? input, out Maybe<object> result)
        {
            OnInteract.Invoke();
            result = Maybe<object>.None;

            return true;
        }
    }
}
