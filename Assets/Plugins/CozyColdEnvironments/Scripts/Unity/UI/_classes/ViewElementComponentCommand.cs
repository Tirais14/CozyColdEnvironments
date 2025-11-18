using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.UI.Elements;
using Humanizer;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.UI
{
    public class ViewElementComponentCommand : CCBehaviourComponentCommand
    {
        [GetBySelf(IsOptional = true)]
        private IViewElement _view = null!;

        protected Maybe<IViewElement> view => _view.Maybe();

        protected override void OnDestroy()
        {
            //view.Match(
            //    some: view => view.IsVisible.Resolve().If(view.Show).Else(view.Hide),
            //    none: () => this.PrintError($"Not found {nameof(ViewElement).Humanize()}.")
            //    );
        }
    }
}
