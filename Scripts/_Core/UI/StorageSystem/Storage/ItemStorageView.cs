#nullable enable
using System;
using System.Collections.Generic;
using UTIRLib.Reflection;

namespace UTIRLib.UI.StorageSystem
{
    public class ItemStorageView<TViewModel, TStorage> : View<TViewModel>
        where TViewModel : ItemStorageViewModel<TStorage>
        where TStorage : ItemStorageUI
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            IItemSlotUI[] slots = GetComponentsInChildren<IItemSlotUI>();

            var createParams = new TypeMemberParameters
            {
                BindingFlags = BindingFlagsDefault.InstanceAll
            };

            var storage = TypeInstanceFactory.Create<TStorage>(createParams,
                new KeyValuePair<Type, object>(typeof(IItemSlotUI[]), slots));

            viewModel = TypeInstanceFactory.Create<TViewModel>(createParams,
                new KeyValuePair<Type, object>(typeof(TStorage), storage));
        }
    }

    public class ItemStorageView : ItemStorageView<ItemStorageViewModel, ItemStorageUI>
    {

    }
}
