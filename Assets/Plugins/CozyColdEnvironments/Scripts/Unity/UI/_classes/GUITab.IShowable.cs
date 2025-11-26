using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUITab : IShowable
    {
        protected readonly HashSet<GraphicStateSnaphsot> hidedComponents = new();

        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_ShowOnInited;

        private Maybe<Graphic> graphic;
        private float graphicColorAlpha;

        public bool ShowOnInited {
            get => m_ShowOnInited;
            set => m_ShowOnInited = value;
        }

        private readonly ReactiveProperty<bool> isVisible = new();

        public bool IsVisible => isVisible.Value;
        public virtual bool ShowAllowed => true;
        private void IShowableStart()
        {
            Show();

            graphic = this.QueryTo()
                .Component<Graphic>()
                .Lax()
                .Map(graphic =>
                {
                    //Does the graphic component transparent until the next frame to resolve flickering when instantiated.
                    graphicColorAlpha = graphic.color.a;
                    graphic.color = graphic.color.WithAlpha(0f);

                    return graphic;
                });

            OnPreUpdateAction(this, 
                static @this =>
                {
                    @this.graphic.IfSome(graphic => graphic.color = graphic.color.WithAlpha(@this.graphicColorAlpha));

                    //if (!@this.ShowOnInited)
                    //    @this.Hide();

                    foreach (var cmp in @this.Q()
                        .ByChildren()
                        .NotRecursive()
                        .Components<IShowable>()
                        .ZL()
                        .Where(cmp => !cmp.ShowOnInited))
                    {
                        cmp.Hide();
                    }
                });
        }

        private void IShowableOnTransformChildrenChanged()
        {
            OnPreUpdateAction(Redraw);
        }

        public virtual void Hide()
        {
            //if (GetPanelParent().Map(gui => !gui.IsVisible).GetValue(false))
            //    return;

            Showable.Hide(gameObject, hidedComponents);
            isVisible.Value = false;
        }

        public virtual void Show()
        {
            //if (GetPanelParent().Map(gui => gui.IsVisible).GetValue(false))
            //    return;
            Showable.Show(gameObject, hidedComponents);
            isVisible.Value = true;
        }

        public bool SwitchVisibleState()
        {
            if (IsVisible)
                Hide();
            else
                Show();

            return IsVisible;
        }

        public void SwitchVisibleStateVoid() => SwitchVisibleState();

        public void Redraw()
        {
            if (IsVisible)
            {
                Hide();
                Show();
            }
            else
            {
                Show();
                Hide();
            }

            this.PrintLog($"{nameof(Redraw)} called.");
        }

        public IObservable<Unit> ObserveShow()
        {
            return isVisible.Where(_ => StartPassed).Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveHide()
        {
            return isVisible.Where(_ => StartPassed).Where(x => !x).AsUnitObservable();
        }
    }
}
