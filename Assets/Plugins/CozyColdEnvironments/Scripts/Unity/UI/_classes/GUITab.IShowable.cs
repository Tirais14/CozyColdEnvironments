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
        private readonly HashSet<GraphicStateSnaphsot> graphicStates = new();
        private readonly HashSet<ShowableStateSnapshot> showableStates = new();

        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_ShowOnInited;

        private float graphicColorAlpha;

        public bool ShowOnInited {
            get => m_ShowOnInited;
            set => m_ShowOnInited = value;
        }

        private readonly ReactiveProperty<bool> isShown = new(true);

        public bool IsShown => isShown.Value;
        public bool IsVisible {
            get => isShown.Value
                   &&
                   enabled
                   &&
                   gameObject.activeSelf
                   &&
                   GetParentGui().Map(gui => gui.IsShown).GetValue(true);
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
            Showable.Hide(gameObject, graphicStates, showableStates);
            isShown.Value = false;
        }

        public virtual void Show()
        {
            Showable.Show(gameObject, graphicStates, showableStates);
            isShown.Value = true;
        }

        public bool SwitchShownState()
        {
            if (IsShown)
                Hide();
            else
                Show();

            return IsShown;
        }

        public void SwitchShownStateVoid() => SwitchShownState();

        public void Redraw()
        {
            if (IsShown)
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
            return isShown.Where(_ => StartPassed).Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveHide()
        {
            return isShown.Where(_ => StartPassed).Where(x => !x).AsUnitObservable();
        }
    }
}
