using CCEnvs.FuncLanguage;
using System;

#nullable enable
namespace CCEnvs.Unity.Storages
{
    public interface IItemContainerInfo : IItemContainerInfoItemless
    {
        Maybe<IItem> Item { get; }

        IObservable<Maybe<IItem>> ObserveItem();
    }
}
