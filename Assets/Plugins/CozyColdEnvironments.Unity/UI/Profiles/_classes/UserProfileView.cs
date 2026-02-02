using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Profiles;
using R3;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

#nullable enable
namespace CCEnvs.Unity.UI.Profiles
{
    public class UserProfileView<TUserProfile> : View<UserProfileViewModel>
    {
        [SerializeField]
        private Image iconView;

        [SerializeField]
        private TMP_Text nameView;

        protected override void Init()
        {
            base.Init();
            BindIcon();
            SetName();
        }

        protected override Maybe<UserProfileViewModel> CreateViewModel()
        {
            return new UserProfileViewModel(UserProfile.Empty);
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
