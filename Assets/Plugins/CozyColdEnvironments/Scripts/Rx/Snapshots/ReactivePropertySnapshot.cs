using CCEnvs.Snapshots;
using R3;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Rx.Snapshots
{
    public record ReactivePropertySnapshot<T> : Snapshot<ReactiveProperty<T>>
    {
        public T? Value { get; set; }

        public ReactivePropertySnapshot()
        {
        }

        public ReactivePropertySnapshot(Snapshot<ReactiveProperty<T>> original) : base(original)
        {
        }

        public ReactivePropertySnapshot(ReactiveProperty<T> target) : base(target)
        {
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReactivePropertySnapshot<T> SetValue(T? value)
        {
            Value = value;
            return this;
        }

        protected override void OnCapture(ReactiveProperty<T> target)
        {
            base.OnCapture(target);
            Value = target.Value;
        }

        protected override void OnRestore(ref ReactiveProperty<T> target)
        {
            if (Value.IsNotNull())
                target.Value = Value;
        }

        protected override void OnReset()
        {
            base.OnReset();
            Value = default;
        }
    }
}
