#nullable enable
using CCEnvs.Snapshots;
using System;

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

        public override string ToString()
        {
            return $"{nameof(IsShown)} \"{IsShown}\"";
        }

        protected override void OnRestore(ref IShowable target)
        {
            if (IsShown)
                target.Show();
            else
                target.Hide();
        }
    }
}
