#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Dependencies;
using R3;
using System;
using YG;

#nullable enable
#pragma warning disable IDE0060
namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    public sealed class YandexAPI : IGeneralAPI
    {
        [field: OnInstallResetable]
        public static YandexAPI? Instance { get; private set; }

        private readonly ReactiveProperty<bool> isInitialized = new();
        private readonly ReactiveProperty<bool> isGameReady = new();
        private readonly ReactiveProperty<bool> isGamePaused = new();
        private readonly ReactiveProperty<bool> isGameWindowShown = new();
        private readonly ReactiveProperty<bool> isGameWindowFocused = new();

        private readonly ReactiveProperty<int> gameplaySession = new();

        private readonly CompositeDisposable disposables = new();

        private Observable<bool>? isGameplayModeObservable;

        private bool isInitializing;

        public bool IsGameplayMode => YG2.isGameplaying;
        public bool IsInitialized => isInitialized.Value;
        public bool IsGameReady => isGameReady.Value;
        public bool IsGamePaused => isGamePaused.Value;
        public bool IsGameWindowShown => isGameWindowShown.Value;
        public bool IsGameWindowFocused => isGameWindowFocused.Value;

        public int GameplaySession => gameplaySession.Value;

        public YandexAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexAPI));

            BindEvents();

            Instance = this;
            CCDependecyContainer.Bind<IGeneralAPI>(this);
            CCDependecyContainer.Bind(this);
        }

        public void GameplayStart()
        {
            YG2.GameplayStart();
            gameplaySession.Value++;
        }

        public void GameplayStop()
        {
            YG2.GameplayStop();
        }

        public void Initialize()
        {
            if (IsInitialized || isInitializing)
                return;

            isInitializing = true;

            try
            {
                YG2.StartInit();

                isInitialized.Value = true;
            }
            catch (Exception)
            {
                isInitialized.Value = false;

                throw;
            }
            finally
            {
                isInitializing = false;
            }

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

            CCDependecyContainer.Unbind<IGeneralAPI>();
            CCDependecyContainer.Unbind(GetType());

            UnbindEvents();

            isInitialized.Dispose();
            isGameReady.Dispose();
            isGamePaused.Dispose();
            isGameWindowShown.Dispose();
            isGameWindowFocused.Dispose();

            gameplaySession.Dispose();

            disposables.DisposeEachAndClear(bufferized: false);

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

            return Observable.Return(IsGameplayMode).Concat(isGameplayModeObservable);
        }

        public Observable<bool> ObserveGamePaused()
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

        public Observable<int> ObserveGameplaySession()
        {
            return gameplaySession;
        }

        private void OnPauseGame(bool state)
        {
            isGamePaused.Value = state;
        }

        private void OnHideWindowGame()
        {
            isGameWindowShown.Value = false;
        }

        private void OnShowWindowGame()
        {
            isGameWindowShown.Value = true;
        }

        private void OnFocusWindowGame(bool state)
        {
            isGameWindowFocused.Value = state;
        }

        private void BindEvents()
        {
            YG2.onPauseGame += OnPauseGame;
            YG2.onHideWindowGame += OnHideWindowGame;
            YG2.onShowWindowGame += OnShowWindowGame;
            YG2.onFocusWindowGame += OnFocusWindowGame;
        }

        private void UnbindEvents()
        {
            YG2.onPauseGame -= OnPauseGame;
            YG2.onHideWindowGame -= OnHideWindowGame;
            YG2.onShowWindowGame -= OnShowWindowGame;
            YG2.onFocusWindowGame -= OnFocusWindowGame;
        }
    }
}
#endif
