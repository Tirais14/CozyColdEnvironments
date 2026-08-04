using R3;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public interface IContextMenuItemViewModel : IViewModel
    {
        ReadOnlyReactiveProperty<string> Name { get; }
    }
}
