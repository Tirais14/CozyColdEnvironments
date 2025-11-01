using CCEnvs.FuncLanguage;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.Storages
{
    public interface IItemContainerInfo : IItemContainerInfoItemless
    {
        IReadOnlyReactiveProperty<Maybe<IItem>> Item { get; }
    }
}
