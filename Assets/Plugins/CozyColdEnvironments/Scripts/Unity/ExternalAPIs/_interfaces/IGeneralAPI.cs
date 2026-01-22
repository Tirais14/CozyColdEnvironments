#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.ExternalAPIs.Yandex;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;

namespace CCEnvs.Unity.ExternalAPIs
{
    public interface IGeneralAPI : IDisposable
    {
        Maybe<IPlayerAPI> PlayerAPI { get; }
        Maybe<IAdvertisementAPI> AdvertisementAPI { get; }

        bool IsInitialized { get; }
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

        UniTask SaveGameAsync(string? serializedData = null, CancellationToken cancellationToken = default);

        UniTask LoadGameAsync(CancellationToken cancellationToken = default);

        Observable<bool> ObserveIsInitialized();

        Observable<bool> ObserveIsGameplayMode();

        Observable<bool> ObserveIsGamePaused();

        Observable<bool> ObserveIsGameReady();

        Observable<bool> ObserveIsGameWindowShown();

        Observable<bool> ObserveIsGameWindowFocused();

        Observable<bool> ObserveIsGameSaving();
    }
}
