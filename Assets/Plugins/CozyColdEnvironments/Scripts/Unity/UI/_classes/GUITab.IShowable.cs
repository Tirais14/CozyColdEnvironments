using CCEnvs.Diagnostics;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
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

        private float graphicColorAlpha;

        public bool ShowOnInited {
            get => m_ShowOnInited;
            set => m_ShowOnInited = value;
        }

        private readonly ReactiveProperty<bool> isVisible = new(true);

        public bool IsVisible {
            get => isVisible.Value
                   &&
                   enabled
                   &&
                   gameObject.activeSelf
                   &&
                   GetParentGUI().Map(gui => gui.IsVisible).GetValue(true);
        }
        public virtual bool ShowAllowed => true;
        public virtual bool HideAllowed => StartPassed;
        private void IShowableStart()
        {
            if (m_Graphic != null)
            {
                graphicColorAlpha = m_Graphic.color.a;
                m_Graphic.color = m_Graphic.color.WithAlpha(0f);
            }

            OnPreUpdateAction(this,
                static @this =>
                {
                    if (@this.m_Graphic != null)
                        @this.m_Graphic.color = @this.m_Graphic.color.WithAlpha(@this.graphicColorAlpha);

                    if (!@this.ShowOnInited)
                        @this.Hide();
                });
        }

        private void IShowableOnTransformChildrenChanged()
        {
            OnPreUpdateAction(Redraw);
        }

        public virtual void Hide()
        {
            Showable.Hide(gameObject, hidedComponents);
            isVisible.Value = false;
        }

        public virtual void Show()
        {
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
