#if YandexGamesPlatform_yg && WEBGL
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R3;
using System;
using UnityEditor;
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

        private readonly CompositeDisposable disposables = new();

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

            BindEvents();
            BindSavingSystem();

            PlayerAPI = playerAPI;
            AdvertisementAPI = advertisementAPI;

            Instance = this;
        }

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        public static void OnEnterPlayMode()
        {
            Instance = null;
        }
#endif

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

            disposables.Dispose();

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

        private void BindEvents()
        {
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
        }

        private void BindSavingSystem()
        {
            SavingSystem.Self.ObserveSaveData()
                .Where(this, static (_, @this) => @this.isInitialized.Value)
                .Subscribe(
                saveData =>
                {
                    var serializedSaveData = JsonConvert.SerializeObject(saveData, CC.JsonSettings);
                    YG2.saves.serializedData = serializedSaveData;
                })
                .AddTo(disposables);
        }
    }
}
#endif
