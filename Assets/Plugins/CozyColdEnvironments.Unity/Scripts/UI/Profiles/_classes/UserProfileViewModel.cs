using System.Threading;
using CCEnvs.Unity.Profiles;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Profiles
{
    public class UserProfileViewModel : ViewModel<IUserProfile>
    {
        public ReadOnlyReactiveProperty<Sprite?> Icon { get; }
        public string Name => GuardedModel.Name;

        public UserProfileViewModel(
            IUserProfile model
            )
        {
            Icon = model.ObserveIcon()
                .ToReadOnlyReactiveProperty(model.Icon)
                .AddTo(ModelDisposables);
        }
    }
}
