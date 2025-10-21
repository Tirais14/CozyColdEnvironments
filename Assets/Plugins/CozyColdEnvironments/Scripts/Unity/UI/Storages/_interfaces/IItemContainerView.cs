using CCEnvs.Unity.GameSystems.Storages;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public interface IItemContainerView<TViewModel, TContainer>
        : IView<TViewModel, TContainer>

        where TViewModel : IItemContainerViewModel<TContainer>
        where TContainer : IItemContainerInfo
    {
    
    }
}
