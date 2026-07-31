using CCEnvs.UnityX.ComponentInjections;
using CCEnvs.UnityX.Components;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.UnityX.UI
{
    public class ViewComponentCommand<T> : CCBehaviourComponentCommand
        where T : class, IView
    {
        [field: GetBySelf(IsOptional = true)]
        protected T view { get; private set; } = null!;
    }
    public class ViewComponentCommand : ViewComponentCommand<IView>
    {

    }
}
