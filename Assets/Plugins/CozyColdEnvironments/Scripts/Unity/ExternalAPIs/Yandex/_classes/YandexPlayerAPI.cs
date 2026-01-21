using R3;
using UnityEngine;
using YG;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public class YandexPlayerAPI : IPlayerAPI
    {
        private Observable<bool>? isAuthorizedObservable;

        public bool IsAuthorized => YG2.player.auth;

        public void Authorize()
        {
#if Authorization_yg
            YG2.OpenAuthDialog();
#endif
        }

        public Observable<bool> ObserveIsAuthorised()
        {
            isAuthorizedObservable ??= Observable.EveryValueChanged((object)null!,
                static _ =>
                {
                    return YG2.player.auth;
                });

            return isAuthorizedObservable;
        }
    }
}
