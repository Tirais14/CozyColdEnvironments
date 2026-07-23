using CCEnvs.Services;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.ComponentInjections;
using R3;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.UnityX.CommonAPIs
{
    public class AuthorizeButtonInactivator : CCBehaviour
    {
        [GetBySelf]
        private Button btn = null!;

        protected override void Start()
        {
            base.Start();

            var playerAPi = CCServices.TryResolve<IPlayerAPI>();

            if (playerAPi.IsNull())
            {
                Destroy(gameObject);
                return;
            }

            playerAPi.ObserveIsAuthorised()
                .Subscribe(this,
                static (state, @this) =>
                {
                    @this.btn.interactable = !state;

                    if (state)
                        Destroy(@this);
                })
                .AddDisposableTo(this);
        }
    }
}
