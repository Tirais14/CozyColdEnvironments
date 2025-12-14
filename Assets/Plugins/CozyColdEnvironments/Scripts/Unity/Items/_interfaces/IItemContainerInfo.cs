using CCEnvs.FuncLanguage;
using R3;
using System;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IItemContainerInfo : IItemContainerInfoItemless
    {
        Maybe<IItem> Item { get; }

        Observable<Maybe<IItem>> ObserveItem();
    }
}
