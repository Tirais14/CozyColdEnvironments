using UniRx;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IItemContainerInfo : IItemContainerInfoItemless
    {
        IReadOnlyReactiveProperty<IItem?> Item { get; }
    }
}
