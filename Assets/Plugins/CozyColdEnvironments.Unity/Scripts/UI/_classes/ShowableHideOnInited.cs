using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;

#nullable enable
namespace CCEnvs.UnityX.UI
{
    public sealed class ShowableHideOnInited : CCBehaviour
    {
        [GetBySelf]
        private IShowableBase showable = null!;

        protected override void Start()
        {
            base.Start();
            showable.Hide();
        }
    }
}
