#if YandexGamesPlatform_yg
using CCEnvs.FuncLanguage;
using Cysharp.Threading.Tasks;
using R3;
using System.Threading;
using YG;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public sealed class YandexAPI : IGeneralAPI
    {
        public static YandexAPI? Instance { get; private set; }

        private readonly ReactiveProperty<bool> isInitialized = new();
        private readonly ReactiveProperty<bool> isGameReady = new();
        private readonly ReactiveProperty<bool> isGamePaused = new();
        private readonly ReactiveProperty<bool> isGameWindowShown = new();
        private readonly ReactiveProperty<bool> isGameWindowFocused = new();
        private readonly ReactiveProperty<bool> isGameSaving = new();

        private Observable<bool>? isGameplayModeObservable;

        public Maybe<IPlayerAPI> PlayerAPI { get; }
        public Maybe<IAdvertisementAPI> AdvertisementAPI { get; }

        public bool IsGameplayMode => YG2.isGameplaying;

        public bool IsInitialized => isInitialized.Value;
        public bool IsGameReady => isGameReady.Value;
        public bool IsGamePaused => isGamePaused.Value;
        public bool IsGameWindowShown => isGameWindowShown.Value;
        public bool IsGameWindowFocused => isGameWindowFocused.Value;
        public bool IsGameSaving => isGameSaving.Value;

        public YandexAPI(
            YandexPlayerAPI? playerAPI = null,
            YandexAdvertisementAPI? advertisementAPI = null
            )
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexAPI));

            YG2.onPauseGame += (state) =>
            {
                isGamePaused.Value = state;
            };

            YG2.onHideWindowGame += () =>
            {
                isGameWindowShown.Value = false;
            };

            YG2.onShowWindowGame += () =>
            {
                isGameWindowShown.Value = true;
            };

            YG2.onFocusWindowGame += state =>
            {
                isGameWindowFocused.Value = state;
            };

            PlayerAPI = playerAPI;
            AdvertisementAPI = advertisementAPI;

            Instance = this;
        }

        public void GameplayStart()
        {
            YG2.GameplayStart();
        }

        public void GameplayStop()
        {
            YG2.GameplayStop();
        }

        public void Initialize()
        {
            YG2.StartInit();
            isInitialized.Value = true;
        }

        public void PauseGame()
        {
            YG2.PauseGame(true);
        }

        public void UnpauseGame()
        {
            YG2.PauseGame(false);
        }

        public void SetGameReady(bool state)
        {
            if (state)
            {
                YG2.GameReadyAPI();
                isGameReady.Value = true;
            }
        }

        public UniTask SaveGameAsync(string? serializedData = null, CancellationToken cancellationToken = default)
        {
            isGameSaving.Value = true;

            YG2.saves = new SavesYG()
            {
                serializedData = serializedData
            };

            YG2.SaveProgress();

            isGameSaving.Value = false;

            return UniTask.CompletedTask;
        }

        public UniTask LoadGameAsync(CancellationToken cancellationToken = default)
        {
            return UniTask.FromResult(string.Empty);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            PlayerAPI.IfSome(x => x.Dispose());
            AdvertisementAPI.IfSome(x => x.Dispose());

            isGameReady.Dispose();
            isGamePaused.Dispose();
            isGameWindowShown.Dispose();
            isGameWindowFocused.Dispose();

            disposed = true;
        }

        public Observable<bool> ObserveIsInitialized()
        {
            return isInitialized;
        }

        public Observable<bool> ObserveIsGameplayMode()
        {
            isGameplayModeObservable ??= Observable.EveryValueChanged((object)null!,
                static _ =>
                {
                    return YG2.isGameplaying;
                });

            return isGameplayModeObservable;
        }

        public Observable<bool> ObserveIsGamePaused()
        {
            return isGamePaused;
        }

        public Observable<bool> ObserveIsGameReady()
        {
            return isGameReady;
        }

        public Observable<bool> ObserveIsGameWindowShown()
        {
            return isGameWindowShown;
        }

        public Observable<bool> ObserveIsGameWindowFocused()
        {
            return isGameWindowFocused;
        }

        public Observable<bool> ObserveIsGameSaving()
        {
            return isGameSaving;
        }
    }
}
#endif
