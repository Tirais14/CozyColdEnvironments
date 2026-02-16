#nullable enable
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;

namespace CCEnvs.Unity.CommonAPIs
{
    public interface IGeneralAPI : IDisposable
    {
        bool IsInitialized { get; }

        bool IsGameReady { get; }

        bool IsGameplayMode { get; }

        bool IsGamePaused { get; }

        bool IsGameWindowShown { get; }

        bool IsGameWindowFocused { get; }

        int GameplaySession { get; }

        TimeProvider TimeProvider { get; }

        void Initialize();

        void SetGameReady(bool state);

        void GameplayStart();

        void GameplayStop();

        void PauseGame();

        void UnpauseGame();

        Observable<bool> ObserveIsInitialized();

        Observable<bool> ObserveIsGameplayMode();

        Observable<bool> ObserveGamePaused();

        Observable<bool> ObserveGameUnpaused();

        Observable<bool> ObserveIsGameReady();

        Observable<bool> ObserveIsGameWindowShown();

        Observable<bool> ObserveIsGameWindowFocused();

        Observable<int> ObserveGameplaySession();
    }
}
