using CCEnvs.Attributes;
using R3;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public class DefaultGeneralAPI : IGeneralAPI
    {
        [OnInstallResetable]
        public static DefaultGeneralAPI? Instance { get; private set; }

        private readonly ReactiveProperty<bool> isInitialized = new();
        private readonly ReactiveProperty<bool> isGameReady = new();
        private readonly ReactiveProperty<bool> isGameplayMode = new();
        private readonly ReactiveProperty<bool> isGamePaused = new();
        private readonly ReactiveProperty<bool> isGameWindowShown = new();
        private readonly ReactiveProperty<bool> isGameWindowFocused = new();

        public bool IsInitialized => isInitialized.Value;
        public bool IsGameReady => isGameReady.Value;
        public bool IsGameplayMode => isGameplayMode.Value;
        public bool IsGamePaused => isGamePaused.Value;
        public bool IsGameWindowShown => isGameWindowShown.Value;
        public bool IsGameWindowFocused => isGameWindowFocused.Value;

        public DefaultGeneralAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(DefaultGeneralAPI));

            Instance = this;
        }

        public void GameplayStart()
        {
            isGameplayMode.Value = true;
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

            isInitialized.Dispose();
            isGameReady.Dispose();
            isGameplayMode.Dispose();
            isGamePaused.Dispose();
            isGameWindowShown.Dispose();
            isGameWindowFocused.Dispose();

            disposed = true;
        }

        public Observable<bool> ObserveIsInitialized()
        {
            return isInitialized;
        }

        public Observable<bool> ObserveIsGamePaused()
        {
            return isGamePaused;
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
    }
}
