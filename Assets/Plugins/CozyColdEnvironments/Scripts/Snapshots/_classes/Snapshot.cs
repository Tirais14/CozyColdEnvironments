using CCEnvs.FuncLanguage;
using System;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public abstract class Snapshot<T> : ISnapshot
    {
#if UNITY_2017_1_OR_NEWER
        protected Maybe<T> target;
#else
        protected readonly Maybe<T> target;
#endif

        public Maybe<object> Target => target.Raw;

        public Snapshot()
        {
        }

        public Snapshot(T target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            this.target = target;
        }

        public void Restore() => Restore(target.Raw!);

        public abstract void Restore(object target);

        protected TTarget ValidateTarget<TTarget>(object target)
        {
            CC.Guard.IsNotNull(target, nameof(target));
            return target.To<TTarget>();
        }
    }
}
