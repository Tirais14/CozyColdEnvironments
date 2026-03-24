#nullable enable
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable S4144
namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Showable))]
    public partial class GUITab
        :
        CCBehaviour
    {
        [Header("Tab Settings")]
        [Space(8)]

        [SerializeField]
        protected bool switchSelectable = true;

        [field: GetBySelf(IsOptional = true)]
        public Button? button { get; private set; }

        [field: GetBySelf(IsOptional = true)]
        public Selectable? selectable { get; private set; }

        protected override void Start()
        {
            base.Start();
            BindSelectable();
        }

        protected virtual void OnTransformChildrenChanged()
        {
        }

        protected virtual void OnTransformParentChanged()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (button.IsNotNull(out var btn))
                btn.onClick.RemoveAllListeners();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool CanSelected() => true;

        private void BindSelectable()
        {
            if (this.button.IsNull(out Button? button))
                return;

            if (selectable.IsNotNull(out var slct))
                button.onClick.AddListener(onClick);

            void onClick()
            {
                if (!slct.IsSelected
                    &&
                    !CanSelected()
                    )
                {
                    return;
                }

                if (switchSelectable)
                    slct.SwitchSelectionState();
                else
                    slct.DoSelect();
            }
        }
    }
}
