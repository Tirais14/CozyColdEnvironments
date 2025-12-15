#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using UnityEngine;

namespace CCEnvs.Unity.UI
{
    public class ShowableSnapshot : Snapshot<IShowable>
    {
        [SerializeField]
        [JsonInclude]
		[JsonPropertyName("isShown")]
        protected bool IsShown;

        [NonSerialized]
        protected Maybe<GameObject> gameObject;

        public ShowableSnapshot(IShowable target)
            :
            base(target)
        {
            IsShown = target.IsShown;

            if (target.As<Component>().TryGetValue(out var cmp))
                gameObject = cmp.gameObject;
        }

        public override IShowable Restore(IShowable target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            if (IsShown)
                target.Show();

            return target;
        }

        public override string ToString()
        {
            return $"{nameof(Target)}: {Target}; {nameof(IsShown)}: {IsShown}; {nameof(gameObject)}: {gameObject}.";
        }
    }
}
