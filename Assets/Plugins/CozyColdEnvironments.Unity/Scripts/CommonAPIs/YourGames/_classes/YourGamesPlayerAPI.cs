#if YOUR_GAMES_PLUGIN_ENABLED && PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Dependencies;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Profiles;
using Cysharp.Threading.Tasks;
using R3;
using YG;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.YourGames
{
    public sealed class YourGamesPlayerAPI : IPlayerAPI
    {
        [field: OnInstallResetable]
        public static YourGamesPlayerAPI? Instance { get; private set; }

        private readonly IUserProfile unauthorizedProfile = new UserProfile(YG2.player.name, YG2.player.id)
        {
            Icon = UCC.AnonymousProfileImage
        };

        private IUserProfile? authorizedProfile;

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

        public IUserProfile? PlayerPofile {
            get
            {
#if Authorization_yg
                return GetOrCreateUserProfile();
#else
                return null;
#endif
            }
        }

        public YourGamesPlayerAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YourGamesAPI));

            Instance = this;

            CCServices.Bind<IPlayerAPI>(this);
            CCServices.Bind(this);
        }

        public void Authorize()
        {
            if (IsAuthorized)
                return;

#if Authorization_yg
            try
            {
                YG2.OpenAuthDialog();
            }
            catch (System.Exception ex)
            {
                this.PrintException(ex);
            }
#endif
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            CCServices.Unbind<IPlayerAPI>();
            CCServices.Unbind(GetType());

            unauthorizedProfile.Dispose();
            authorizedProfile?.Dispose();

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

        private IUserProfile GetOrCreateUserProfile()
        {
            if (IsAuthorized)
            {
                if (authorizedProfile is null)
                {
                    authorizedProfile = new UserProfile(YG2.player.name, YG2.player.id);

                    YourGamesPluginHelper.LoadImageAsync(YG2.player.photo).
                        ContinueWith((img) =>
                        {
                            authorizedProfile.Icon = img;
                        })
                        .ForgetByPrintException();
                }

                return authorizedProfile;
            }

            return unauthorizedProfile;
        }
    }
}
#endif
