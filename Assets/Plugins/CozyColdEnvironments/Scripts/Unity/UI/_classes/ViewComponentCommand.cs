using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.UI.MVVM;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.UI
{
    public class ViewComponentCommand<T> : CCBehaviourComponentCommand
        where T : class, IGUIPanel
    {
        [field: GetBySelf(IsOptional = true)]
        protected T view { get; private set; } = null!;
    }
    public class ViewComponentCommand : ViewComponentCommand<IGUIPanel>
    {

    }
}
