#nullable enable
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
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
        bool IsGameSaving { get; }
        bool IsSaveGameLoading { get; }

        int GameplaySession { get; }

        void Initialize();

        void SetGameReady(bool state);

        void GameplayStart();

        void GameplayStop();

        void PauseGame();

        void UnpauseGame();

        UniTask SaveGameAsync(string? filePath = null);

        UniTask LoadSaveGameAsync(string? filePath = null);

        Observable<bool> ObserveIsInitialized();

        Observable<bool> ObserveIsGameplayMode();

        Observable<bool> ObserveIsGamePaused();

        Observable<bool> ObserveIsGameReady();

        Observable<bool> ObserveIsGameWindowShown();

        Observable<bool> ObserveIsGameWindowFocused();

        Observable<int> ObserveGameplaySession();

        Observable<bool> ObserveIsGameSaving();

        Observable<bool> ObserveIsSaveGameLoading();
    }
}
