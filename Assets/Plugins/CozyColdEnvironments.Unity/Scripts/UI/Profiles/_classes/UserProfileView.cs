using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI.Profiles
{
    public abstract class UserProfileView : View<UserProfileViewModel>
    {
        [SerializeField]
        private Image iconView;

        [SerializeField]
        private TMP_Text nameView;

        protected override void InitViewModel()
        {
            base.InitViewModel();
            BindIcon();
            SetName();
        }

        private void BindIcon()
        {
            viewModelUnsafe.Icon.Subscribe(this,
                static (icon, @this) =>
                {
                    @this.iconView.sprite = icon;
                })
                .AddTo(viewModelDisposables);
        }

        private void SetName()
        {
            nameView.text = viewModelUnsafe.Name;
        }
    }
}
