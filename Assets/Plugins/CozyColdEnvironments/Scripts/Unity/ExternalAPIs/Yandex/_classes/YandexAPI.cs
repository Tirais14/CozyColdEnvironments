#if YandexGamesPlatform_yg
using R3;
using System;
using YG;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public sealed class YandexAPI : IGeneralAPI
    {
        public static YandexAPI? Instance { get; private set; }

        private readonly CompositeDisposable disposables = new();

        private readonly ReactiveProperty<bool> isGameReady = new();
        private readonly ReactiveProperty<bool> isGamePaused = new();
        private readonly ReactiveProperty<bool> isAdvertisementMode = new();
        private readonly ReactiveProperty<bool> isGameWindowShown = new();
        private readonly ReactiveProperty<bool> isGameWindowFocused = new();

        private Observable<bool>? isGameplayModeObservable;

        public bool IsAuthorized => throw new NotImplementedException();
        public bool IsGameReady => isGameReady.Value;
        public bool IsGameplayMode => YG2.isGameplaying;
        public bool IsGamePaused => isGamePaused.Value;
        public bool IsAdvertisementMode => isAdvertisementMode.Value;
        public bool IsGameWindowShown => isGameWindowShown.Value;

        public YandexAPI()
        {
            if (Instance is not null)
                throw new InvalidOperationException($"Cannot create new instance of {nameof(YandexAPI)}");

            YG2.onPauseGame += (state) =>
            {
                isGamePaused.Value = state;
            };

            YG2.onCloseAnyAdv += () =>
            {
                isAdvertisementMode.Value = false;
            };

            YG2.onOpenAnyAdv += () =>
            {
                isAdvertisementMode.Value = true;
            };

            YG2.onHideWindowGame += () =>
            {
                isGameWindowShown.Value = false;
            };

            YG2.onShowWindowGame += () =>
            {
                isGameWindowShown.Value = true;
            };

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

        public Observable<bool> ObserveIsAdvertisementMode()
        {
            return isAdvertisementMode;
        }

        public Observable<bool> ObserveIsGameWindowShown()
        {
            return isGameWindowShown;
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposables.Dispose();

            isGameReady.Dispose();
            isGamePaused.Dispose();
            isAdvertisementMode.Dispose();
            isGameWindowShown.Dispose();
            isGameWindowFocused.Dispose();

            disposed = true;
        }
    }
}
#endif
