//using CCEnvs.UnityX.Items.UI;
//using CCEnvs.UnityX.UI;
//using UnityEngine;
//using UnityEngine.UIElements;

//#nullable enable
//namespace CCEnvs.UnityX.Items.UIElements
//{
//    public abstract class InventoryView<TViewModel> : View<TViewModel>
//        where TViewModel : IInventoryViewModel
//    {
//        [SerializeField]
//        protected UIDocument uiDocument;

//        [SerializeField]
//        protected GameObject containerPrefab;

//        [SerializeField]
//        protected Transform containersRoot;

//        public GameObject ContainerPrefab {
//            get => containerPrefab;
//            set => containerPrefab = value;
//        }

//        public UIDocument UIDocument {
//            get => uiDocument;
//            set => uiDocument = value;
//        }

//        public Transform? ContainersRoot {
//            get => containersRoot;
//            set => containersRoot = value.IfNull(transform);
//        }

//        protected override void Awake()
//        {
//            base.Awake();
//            containersRoot = containersRoot.IfNull(transform);
//        }
//    }

//    public class InventoryView : InventoryView<InventoryViewModel<Inventory>>
//    {
//        [SerializeField]
//        protected int containerCount;

//        [SerializeField]
//        protected bool containerAutoSize;

//        public int ContainerCount {
//            get => containerCount;
//            set => containerCount = value;
//        }

//        public bool ContainerAutoSize {
//            get => containerAutoSize;
//            set => containerAutoSize = value;
//        }

//        protected override InventoryViewModel<Inventory>? CreateViewModel()
//        {
//            return new InventoryViewModel<Inventory>(
//                new Inventory(),
//                containerPrefab,
//                containersRoot
//                );
//        }
//    }
//}
