#nullable enable
using R3;
using System;

namespace CCEnvs.Unity.ExternalAPI
{
    public interface IExternalAPI : IDisposable
    {
        bool IsAuthorized { get; }
        bool IsGameReady { get; }
        bool IsGameplayMode { get; }
        bool IsGamePaused { get; }
        bool IsAdvertisementMode { get; }
        bool IsGameWindowShown { get; }

        void Initialize();

        void Authorize();

        void SetGameReady(bool state);

        void GameplayStart();

        void GameplayStop();

        void PauseGame();

        void UnpauseGame();

        Observable<bool> ObserveIsAuthorised();

        Observable<bool> ObserveIsGameplayMode();

        Observable<bool> ObserveIsGamePaused();

        Observable<bool> ObserveIsGameReady();

        Observable<bool> ObserveIsAdvertisementMode();

        Observable<bool> ObserveIsGameWindowShown();
    }
}
