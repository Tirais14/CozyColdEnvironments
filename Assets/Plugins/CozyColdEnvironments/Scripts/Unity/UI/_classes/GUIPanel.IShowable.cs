using CCEnvs.Diagnostics;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUIPanel : IShowable
    {
        protected readonly HashSet<GraphicStateSnaphsot> hidedComponents = new();

        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_ShowOnStart;

        public bool ShowOnStart {
            get => m_ShowOnStart;
            set => m_ShowOnStart = value;
        }

        private readonly ReactiveProperty<bool> isVisible = new();

        public bool IsVisible => isVisible.Value;
        public virtual bool ShowAllowed => true;

        private void IShowableStart()
        {
            Show();
            OnPreUpdateAction(this, @this =>
            {
                if (!@this.ShowOnStart && @this.GetParentGUI().IsNone)
                    @this.Hide();
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
