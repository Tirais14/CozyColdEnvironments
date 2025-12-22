#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System;
using UnityEngine;

namespace CCEnvs.Unity.UI
{
    [Serializable]
    public class ShowableSnapshot : Snapshot<IShowable>
    {
        public bool isShown { get; set; }

        public override bool IgnoreTarget => false;

        [NonSerialized]
        protected Maybe<GameObject> gameObject;

        public ShowableSnapshot(IShowable target)
            :
            base(target)
        {
            isShown = target.IsShown;

            if (target.As<Component>().TryGetValue(out var cmp))
                gameObject = cmp.gameObject;
        }

        public override Maybe<IShowable> Restore(IShowable? target)
        {
            if (target.IsNull())
                return Maybe<IShowable>.None;

            if (isShown)
                target.Show();

            return target.Maybe();
        }

        public override string ToString()
        {
            return $"{nameof(Target)}: {Target}; {nameof(isShown)}: {isShown}; {nameof(gameObject)}: {gameObject}.";
        }
    }
}
