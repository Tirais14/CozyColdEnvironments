#nullable enable
using R3;
using System;

namespace CCEnvs.Unity.ExternalAPIs
{
    public interface IGeneralAPI : IDisposable
    {
        bool IsInitialized { get; }
        bool IsGameReady { get; }
        bool IsGameplayMode { get; }
        bool IsGamePaused { get; }
        bool IsGameWindowShown { get; }
        bool IsGameWindowFocused { get; }

        void Initialize();

        void SetGameReady(bool state);

        void GameplayStart();

        void GameplayStop();

        void PauseGame();

        void UnpauseGame();

        Observable<bool> ObserveIsInitialized();

        Observable<bool> ObserveIsGameplayMode();

        Observable<bool> ObserveIsGamePaused();

        Observable<bool> ObserveIsGameReady();

        Observable<bool> ObserveIsGameWindowShown();

        Observable<bool> ObserveIsGameWindowFocused();
    }
}
