#nullable enable
using CCEnvs.FuncLanguage;
using UnityEngine;

namespace CCEnvs.Unity.UI
{
    public readonly struct ShowableStateSnapshot : ISnapshot<IShowable>
    {
        public IShowable Target { get; }
        public bool IsShown { get; }
        public Maybe<GameObject> gameObject { get; }

        public ShowableStateSnapshot(IShowable target)
            :
            this()
        {
            CC.Guard.IsNotNull(target, nameof(target));

            Target = target;
            IsShown = target.IsShown;

            if (target.AsOrDefault<Component>().TryGetValue(out var cmp))
                gameObject = cmp.gameObject;
        }

        public void Restore()
        {
            if (IsShown)
                Target.Show();
        }

        public override string ToString()
        {
            return $"{nameof(Target)}: {Target}; {nameof(IsShown)}: {IsShown}; {nameof(gameObject)}: {gameObject}.";
        }
    }
}
