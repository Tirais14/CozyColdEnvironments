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

        protected override void InitViewModel(UserProfileViewModel vm)
        {
            base.InitViewModel(vm);
            BindIcon(vm);
            SetName(vm);
        }

        private void BindIcon(UserProfileViewModel vm)
        {
            vm.Icon.Subscribe(this,
                static (icon, @this) =>
                {
                    @this.iconView.sprite = icon;
                })
                .AddTo(ViewModelDisposables);
        }

        private void SetName(UserProfileViewModel vm)
        {
            nameView.text = vm.Name;
        }
    }
}
