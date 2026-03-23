using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using CommunityToolkit.Diagnostics;
using ObservableCollections;
using System;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public class InventoryViewModel<TModel>
        :
        ViewModel<TModel>, 
        IInventoryViewModel

        where TModel : IInventory
    {
        private readonly Func<IItemContainerViewModel> cntViewModelFactory;

        private IDisposable? addContainerBinding;
        private IDisposable? removeContainerBinding;
        private IDisposable? replaceContainerBinding;

        public IReadOnlyObservableDictionary<int, IItemContainer> Containers => Model.Containers;

        public GameObject ContainerPrefab { get; }

        public InventoryViewModel(
            TModel model,
            CancellationToken cancellationToken,
            GameObject containerPrefab,
            Func<IItemContainerViewModel> cntViewModelFactory
            )
            :
            base(model, cancellationToken)
        {
            CC.Guard.IsNotNull(containerPrefab, nameof(containerPrefab));
            Guard.IsNotNull(cntViewModelFactory, nameof(cntViewModelFactory));

            ContainerPrefab = containerPrefab;
            this.cntViewModelFactory = cntViewModelFactory;
        }

        public void OnAddContainer(ItemContainerView cntView)
        {
            CC.Guard.IsNotNull(cntView, nameof(cntView));


        }
    }
}
