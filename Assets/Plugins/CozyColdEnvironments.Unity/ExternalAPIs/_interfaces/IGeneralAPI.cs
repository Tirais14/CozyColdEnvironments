#nullable enable
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;

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

        TimeProvider TimeProvider { get; }

        void Initialize();

        void SetGameReady(bool state);

        void GameplayStart();

        void GameplayStop();

        void PauseGame();

        void UnpauseGame();

        UniTask SaveGameAsync(string? filePath = null, CancellationToken cancellationToken = default);

        UniTask LoadSaveGameAsync(string? filePath = null, CancellationToken cancellationToken = default);

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
