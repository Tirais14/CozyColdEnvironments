#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.ExternalAPIs.Yandex;
using R3;
using System;

namespace CCEnvs.Unity.ExternalAPIs
{
    public interface IGeneralAPI : IDisposable
    {
        Maybe<IPlayerAPI> PlayerAPI { get; }
        Maybe<IAdvertisementAPI> AdvertisementAPI { get; }

        bool IsGameReady { get; }
        bool IsGameplayMode { get; }
        bool IsGamePaused { get; }
        bool IsGameWindowShown { get; }
        bool IsGameWindowFocused { get; }
        bool IsGameSaving { get; }

        void Initialize();

        void SetGameReady(bool state);

        void GameplayStart();

        void GameplayStop();

        void PauseGame();

        void UnpauseGame();

        void SaveGame(string serializedData);

        Observable<bool> ObserveIsGameplayMode();

        Observable<bool> ObserveIsGamePaused();

        Observable<bool> ObserveIsGameReady();

        Observable<bool> ObserveIsGameWindowShown();

        Observable<bool> ObserveIsGameWindowFocused();

        Observable<bool> ObserveIsGameSaving();
    }
}
