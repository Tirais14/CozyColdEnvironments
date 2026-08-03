//using CCEnvs.Disposables;
//using CCEnvs.UnityX.ComponentInjections;
//using CCEnvs.UnityX.Components;
//using R3;
//using System;
//using UnityEngine.UIElements;

//#nullable enable
//namespace CCEnvs.UnityX.UI.Elements
//{
//    public class ButtonForce : CCBehaviour
//    {
//        [GetBySelf]
//        private IElement element = null!;

//        private IDisposable? rootElementBinding;

//        private LightDisposable<(ButtonForce, VisualElement)> pointerDownRegistration;
//        private LightDisposable<(ButtonForce, VisualElement)> pointerMoveRegistration;
//        private LightDisposable<(ButtonForce, VisualElement)> pointerUpRegistration;

//        protected override void Start()
//        {
//            base.Start();
//            rootElementBinding = element.ObserveRootElement()
//                .Subscribe(OnRootElementChanged);
//        }

//        protected override void OnDestroy()
//        {
//            base.OnDisable();
//            CCDisposable.Dispose(ref rootElementBinding);
//        }

//        protected virtual void OnPointerDown(PointerDownEvent ev) { }

//        private void OnPointerDownInternal(PointerDownEvent ev)
//        {

//            Button
//            try
//            {
//                OnPointerDown(ev);
//            }
//            catch (Exception ex)
//            {
//                this.PrintException(ex);
//            }
//        }

//        private void OnPointerMove

//        private void OnRootElementChanged(VisualElement? root)
//        {
//            if (root is not null)
//            {
//                root.RegisterCallback<PointerDownEvent>(OnPointerDownInternal);
//                root.RegisterCallback<PointerMoveEvent>(OnPointer)

//            }
//        }
//    }
//}
