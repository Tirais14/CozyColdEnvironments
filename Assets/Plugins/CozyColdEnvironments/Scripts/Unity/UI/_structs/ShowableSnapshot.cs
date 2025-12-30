#nullable enable
using CCEnvs.Snapshots;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Unity.UI
{
    [Serializable]
    public class ShowableSnapshot : Snapshot<IShowable>
    {
        public bool IsShown { get; private set; }

        public ShowableSnapshot(IShowable target)
            :
            base(target)
        {
            IsShown = target.IsShown;
        }

        public override bool Restore(IShowable? target, [NotNullWhen(true)] out IShowable? restored)
        {
            if (!CanRestore(target))
            {
                restored = null;
                return false;
            }

            if (IsShown)
                target.Show();

            restored = target;
            return true;
        }

        public override string ToString()
        {
            return $"{nameof(IsShown)} \"{IsShown}\"";
        }
    }
}
