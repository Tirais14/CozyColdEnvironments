#if YandexGamesPlatform_yg
using CommunityToolkit.Diagnostics;
using R3;
using System;
using YG;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public sealed class YandexAPI : IGeneralAPI
    {
        public static YandexAPI? Instance { get; private set; }

        private readonly ReactiveProperty<bool> isGameReady = new();
        private readonly ReactiveProperty<bool> isGamePaused = new();
        private readonly ReactiveProperty<bool> isAdvertisementMode = new();
        private readonly ReactiveProperty<bool> isGameWindowShown = new();

        private Observable<bool>? isGameplayModeObservable;
        private Observable<bool>? isGameWindowFocused;

        public IPlayerAPI PlayerAPI { get; }
        public bool IsAuthorized => throw new NotImplementedException();
        public bool IsGameReady => isGameReady.Value;
        public bool IsGameplayMode => YG2.isGameplaying;
        public bool IsGamePaused => isGamePaused.Value;
        public bool IsAdvertisementMode => isAdvertisementMode.Value;
        public bool IsGameWindowShown => isGameWindowShown.Value;
        public bool IsGameWindowFocused => YG2.isFocusWindowGame;

        public YandexAPI(YandexPlayerAPI playerAPI)
        {
            Guard.IsNotNull(playerAPI, nameof(playerAPI));

            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexAPI));

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

            PlayerAPI = playerAPI;

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

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            isGameReady.Dispose();
            isGamePaused.Dispose();
            isAdvertisementMode.Dispose();
            isGameWindowShown.Dispose();

            disposed = true;
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

        public Observable<bool> ObserveIsGameWindowFocused()
        {
            isGameWindowFocused ??= Observable.EveryValueChanged((object)null!,
                static _ =>
                {
                    return YG2.isFocusWindowGame;
                });

            return isGameWindowFocused;
        }
    }
}
#endif
