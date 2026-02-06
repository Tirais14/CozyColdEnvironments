using CCEnvs.Unity.Profiles;
using R3;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Profiles
{
    public class UserProfileViewModel : ViewModel<IUserProfile>
    {
        public ReadOnlyReactiveProperty<Sprite?> Icon { get; }
        public string Name => model.Name;

        public UserProfileViewModel(
            IUserProfile model, 
            CancellationToken cancellationToken = default
            ) 
            : 
            base(model, cancellationToken)
        {
            Icon = model.ObserveIcon()
                .ToReadOnlyReactiveProperty(model.Icon)
                .AddTo(disposables);
        }
    }
}
