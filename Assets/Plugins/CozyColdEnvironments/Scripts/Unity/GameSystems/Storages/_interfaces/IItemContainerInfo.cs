using CCEnvs.FuncLanguage;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages
{
    public interface IItemContainerInfo : IItemContainerInfoItemless
    {
        IReadOnlyReactiveProperty<Maybe<IItem>> Item { get; }
    }
}
