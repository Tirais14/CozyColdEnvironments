using CCEnvs.FuncLanguage;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public abstract class Snapshot<T> : ISnapshot<T>
    {
#if UNITY_2017_1_OR_NEWER
        [SerializeField]
        protected Maybe<T> target;
#else
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
