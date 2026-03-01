#nullable enable
using CCEnvs.Attributes.Serialization;
using CCEnvs.Snapshots;
using System;

namespace CCEnvs.Unity.UI
{
    [Serializable]
    public record ShowableSnapshot<T> : Snapshot<T>
        where T : IShowable
    {
        public bool? IsShown { get; private set; }

        public ShowableSnapshot()
            :
            base()
        {
        }

        public ShowableSnapshot(T target)
            :
            base(target)
        {
        }

        public ShowableSnapshot(Snapshot<T> original) : base(original)
        {
        }

        public override string ToString()
        {
            return $"{nameof(IsShown)} \"{IsShown}\"";
        }

        protected override void OnRestore(ref T target)
        {
            if (IsShown.HasValue)
            {
                if (IsShown.Value)
                    target.Show();
                else
                    target.Hide();
            }
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            IsShown = target.IsShown;
        }

        protected override void OnReset()
        {
            base.OnReset();

            IsShown = default;
        }
    }

    [Serializable]
    [SerializationDescriptor("ShowableSnapshot", "9c03d67d-e30b-4531-8881-add1c5c12420")]
    public record ShowableSnapshot : ShowableSnapshot<IShowable>
    {
        public ShowableSnapshot()
        {
        }

        public ShowableSnapshot(IShowable target) : base(target)
        {
        }

        public ShowableSnapshot(Snapshot<IShowable> original) : base(original)
        {
        }

        protected ShowableSnapshot(ShowableSnapshot<IShowable> original) : base(original)
        {
        }
    }
}
