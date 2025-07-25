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

            var createParams = new ConstructorParameters
            {
                BindingFlags = BindingFlagsDefault.InstanceAll,
            };

            var storage = InstanceFactory.Create<TStorage>(
                createParams with
                {
                    ArgumentsData = new KeyValuePair<Type, object>[]
                    {
                        new(typeof(IItemSlotUI[]), slots)
                    } 
                },
                cacheResults: false);

            viewModel = InstanceFactory.Create<TViewModel>(createParams with
            {
                ArgumentsData = new KeyValuePair<Type, object>[] 
                {
                    new(typeof(TStorage), storage) 
                }
            },
            cacheResults: false);
        }
    }

    public class ItemStorageView : ItemStorageView<ItemStorageViewModel, ItemStorageUI>
    {

    }
}
