#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using UnityEngine;

namespace CCEnvs.Unity.UI
{
    public class ShowableSnapshot : Snapshot<IShowable>
    {
        public bool IsShown { get; }
        public Maybe<GameObject> gameObject { get; }

        public ShowableSnapshot(IShowable target)
            :
            base(target)
        {
            IsShown = target.IsShown;

            if (target.As<Component>().TryGetValue(out var cmp))
                gameObject = cmp.gameObject;
        }

        public override void Restore(object target)
        {
            var showable = ValidateTarget<IShowable>(target);

            if (IsShown)
                showable.Show();
        }

        public override string ToString()
        {
            return $"{nameof(Target)}: {Target}; {nameof(IsShown)}: {IsShown}; {nameof(gameObject)}: {gameObject}.";
        }
    }
}
