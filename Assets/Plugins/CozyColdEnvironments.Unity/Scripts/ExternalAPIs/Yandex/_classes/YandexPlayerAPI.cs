#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Profiles;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using System;
using System.Collections.Generic;
using YG;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public sealed class YandexPlayerAPI : IPlayerAPI
    {
        [field: OnInstallResetable]
        public static YandexPlayerAPI? Instance { get; private set; }


        private readonly List<IDisposable> disposables = new();

#if Authorization_yg
        private Maybe<ImageLoadYG> imageLoadCmp;

        private Observable<bool>? isAuthorizedObservable;
#endif

        private IUserProfile? profile;

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

        public IUserProfile? Profile {
            get
            {
#if Authorization_yg
                if (IsAuthorized)
                {
                    if (profile.IsNotNull())
                        return profile;

                    profile = new UserProfile(YG2.player.name, YG2.player.id);

                    YandexPluginHelper.LoadImageAsync(YG2.player.photo).
                        ContinueWith((img) =>
                        {
                            profile.Icon = img;
                        })
                        .ForgetByPrintException();

                    return profile;
                }

                return null;
#else
                return null;
#endif
            }
        }

        public YandexPlayerAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexAPI));

#if Authorization_yg
            imageLoadCmp = GameObjectQuery.Scene.IncludeInactive()
                .Component<ImageLoadYG>()
                .Lax();
#endif

            Instance = this;
        }

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
