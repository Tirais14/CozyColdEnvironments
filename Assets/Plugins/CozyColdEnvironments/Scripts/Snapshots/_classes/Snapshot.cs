using CCEnvs.FuncLanguage;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public abstract class Snapshot<T> : ISnapshot<T>
    {
#if UNITY_2017_1_OR_NEWER
        [JsonIgnore]
        [SerializeField]
        protected Maybe<T> target;
#else
        [JsonIgnore]
        protected readonly Maybe<T> target;
#endif

        public Maybe<T> Target => target.Raw;

        public Snapshot()
        {
        }

        public Snapshot(T target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            this.target = target;
        }

        public T Restore() => Restore(target.Raw!);

        public abstract T Restore(T target);

        protected TTarget ValidateTarget<TTarget>(T target)
        {
            CC.Guard.IsNotNull(target, nameof(target));
            return target.To<TTarget>();
        }
    }
}
