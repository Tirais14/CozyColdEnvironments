#if YandexGamesPlatform_yg && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R3;
using System;
using YG;

#nullable enable
#pragma warning disable IDE0060
namespace CCEnvs.Unity.ExternalAPIs.Yandex
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

        public TimeProvider TimeProvider { get; }

        public YandexAPI(TimeProvider? timeProvider = null)
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexAPI));

            TimeProvider = timeProvider ?? UnityTimeProvider.Update;

            BindEvents();
            BindSavingSystem();

            Instance = this;
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
            if (IsInitialized)
                throw new InvalidOperationException("Already initialized");

            if (isInitializing)
                throw new InvalidOperationException("Already initializing");

            isInitializing = true;

            YG2.StartInit();

            UniTask.Create(this,
                static async @this =>
                {
                    await SavingSystem.Self.LoadFromSerializedData(YG2.saves.serializedData);

                    //some delay between initialized
                    await UniTask.WaitForSeconds(1f);

                    @this.isInitialized.Value = true;
                    @this.isInitializing = false;
                })
                .Forget();
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

            YG2.onPauseGame -= OnPauseGame;
            YG2.onHideWindowGame -= OnHideWindowGame;
            YG2.onShowWindowGame -= OnShowWindowGame;
            YG2.onFocusWindowGame -= OnFocusWindowGame;

            isInitialized.Dispose();
            isGameReady.Dispose();
            isGamePaused.Dispose();
            isGameWindowShown.Dispose();
            isGameWindowFocused.Dispose();

            gameplaySession.Dispose();

            disposables.Dispose();
            disposables.Clear();

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

        public Observable<bool> ObserveGamePaused()
        {
            return isGamePaused.Where(static x => x);
        }

        public Observable<bool> ObserveGameUnpaused()
        {
            return isGamePaused.Where(static x => !x);
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

        private void BindSavingSystem()
        {
            SavingSystem.Self.ObserveSaveData()
                .Where(this, static (_, @this) => @this.isInitialized.Value)
                .Subscribe(this,
                static (saveData, @this) =>
                {
                    var serializedSaveData = JsonConvert.SerializeObject(saveData, CC.JsonSettings);
                    YG2.saves.serializedData = serializedSaveData;
                    YG2.SaveProgress();
                })
                .AddTo(disposables);
        }
    }
}
#endif
