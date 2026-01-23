#if YandexGamesPlatform_yg && WEBGL
using R3;
using UnityEditor;
using YG;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public sealed class YandexPlayerAPI : IPlayerAPI
    {
        public static YandexPlayerAPI? Instance { get; private set; }

#if Authorization_yg
        private Observable<bool>? isAuthorizedObservable;
#endif

        public bool IsAuthorized {
            get
            {
#if Authorization_yg
                return YG2.player.auth;
#else
                return false;
#endif
            }
        }

        public YandexPlayerAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexAPI));

            Instance = this;
        }

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        public static void OnEnterPlayMode()
        {
            Instance = null;
        }
#endif

        public void Authorize()
        {
#if Authorization_yg
            YG2.OpenAuthDialog();
#endif
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
        }

        public Observable<bool> ObserveIsAuthorised()
        {
#if Authorization_yg
            isAuthorizedObservable ??= Observable.EveryValueChanged((object)null!,
                static _ =>
                {
                    return YG2.player.auth;
                });

            return isAuthorizedObservable;
#else
            return Observable.Empty<bool>();
#endif
        }
    }
}
#endif
