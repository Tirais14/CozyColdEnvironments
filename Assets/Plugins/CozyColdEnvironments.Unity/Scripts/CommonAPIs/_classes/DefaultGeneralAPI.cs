using CCEnvs.Attributes;
using CCEnvs.Dependencies;
using R3;
using System;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    public class DefaultGeneralAPI : IGeneralAPI
    {
        [field: OnInstallResetable]
        public static DefaultGeneralAPI? Instance { get; private set; }

        private readonly ReactiveProperty<bool> isInitialized = new();
        private readonly ReactiveProperty<bool> isGameReady = new();
        private readonly ReactiveProperty<bool> isGameplayMode = new();
        private readonly ReactiveProperty<bool> isGamePaused = new();
        private readonly ReactiveProperty<bool> isGameWindowShown = new();
        private readonly ReactiveProperty<bool> isGameWindowFocused = new();

        private readonly ReactiveProperty<int> gameplaySession = new();

        public bool IsInitialized => isInitialized.Value;
        public bool IsGameReady => isGameReady.Value;
        public bool IsGameplayMode => isGameplayMode.Value;
        public bool IsGamePaused => isGamePaused.Value;
        public bool IsGameWindowShown => isGameWindowShown.Value;
        public bool IsGameWindowFocused => isGameWindowFocused.Value;

        public int GameplaySession => gameplaySession.Value;

        public TimeProvider TimeProvider { get; }

        public DefaultGeneralAPI(TimeProvider? timeProvider = null)
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(DefaultGeneralAPI));

            TimeProvider = timeProvider ?? UnityTimeProvider.Update;

            Instance = this;

            CCDependecyContainer.Bind<IGeneralAPI>(this);
            CCDependecyContainer.Bind(this);
        }

        public void GameplayStart()
        {
            isGameplayMode.Value = true;
            gameplaySession.Value++;
        }

        public void GameplayStop()
        {
            isGameplayMode.Value = false;
        }

        public void Initialize()
        {
            isInitialized.Value = true;
        }

        public void PauseGame()
        {
            isGamePaused.Value = true;
        }

        public void UnpauseGame()
        {
            isGamePaused.Value = false;
        }

        public void SetGameReady(bool state)
        {
            isGameReady.Value = state;
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            CCDependecyContainer.Unbind<IGeneralAPI>();
            CCDependecyContainer.Unbind(GetType());

            isInitialized.Dispose();
            isGameReady.Dispose();
            isGameplayMode.Dispose();
            isGamePaused.Dispose();
            isGameWindowShown.Dispose();
            isGameWindowFocused.Dispose();

            gameplaySession.Dispose();

            disposed = true;
        }

        public Observable<bool> ObserveIsInitialized()
        {
            return isInitialized;
        }

        public Observable<bool> ObserveGamePaused()
        {
            return isGamePaused.Where(static x => x);
        }

        public Observable<bool> ObserveGameUnpaused()
        {
            return isGamePaused.Where(static x => !x);
        }

        public Observable<bool> ObserveIsGameplayMode()
        {
            return isGameplayMode;
        }

        public Observable<bool> ObserveIsGameReady()
        {
            return isGameReady;
        }

        public Observable<bool> ObserveIsGameWindowFocused()
        {
            return isGameWindowFocused;
        }

        public Observable<bool> ObserveIsGameWindowShown()
        {
            return isGameWindowShown;
        }

        public Observable<int> ObserveGameplaySession()
        {
            return gameplaySession;
        }
    }
}
