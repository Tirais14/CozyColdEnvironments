#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace CCEnvs.Unity.UI
{
    [Serializable]
    public class ShowableSnapshot : Snapshot<IShowable>
    {
        [JsonInclude]
        [SerializeField]
        protected bool isShown;

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

        public ShowableSnapshot(bool isShown)
        {
            this.isShown = isShown;
        }

        public override IShowable Restore(IShowable? target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            if (isShown)
                target.Show();

            return target;
        }

        public override string ToString()
        {
            return $"{nameof(Target)}: {Target}; {nameof(isShown)}: {isShown}; {nameof(gameObject)}: {gameObject}.";
        }
    }
}
