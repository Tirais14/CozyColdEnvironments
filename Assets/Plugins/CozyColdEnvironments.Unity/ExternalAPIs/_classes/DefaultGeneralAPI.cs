using CCEnvs.Attributes;
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;

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

        private readonly ReactiveProperty<int> gameplaySession = new();

        public bool IsInitialized => isInitialized.Value;
        public bool IsGameReady => isGameReady.Value;
        public bool IsGameplayMode => isGameplayMode.Value;
        public bool IsGamePaused => isGamePaused.Value;
        public bool IsGameWindowShown => isGameWindowShown.Value;
        public bool IsGameWindowFocused => isGameWindowFocused.Value;
        public bool IsGameSaving => SavingSystem.Self.IsSaving;
        public bool IsSaveGameLoading => SavingSystem.Self.IsSaveLoading;

        public int GameplaySession => gameplaySession.Value;

        public TimeProvider TimeProvider { get; }

        public DefaultGeneralAPI(TimeProvider? timeProvider = null)
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(DefaultGeneralAPI));

            TimeProvider = timeProvider ?? UnityTimeProvider.Update;

            Instance = this;
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

        public async UniTask SaveGameAsync(
            string? filePath = null,
            CancellationToken cancellationToken = default
            )
        {
            if (filePath.IsNullOrWhiteSpace())
            {
                await SavingSystem.Self.SaveInMemoryAsync(cancellationToken);
                return;
            }

            await SavingSystem.Self.SaveInFileAsync(filePath, cancellationToken);
        }

        public async UniTask LoadSaveGameAsync(
            string? filePath = null,
            CancellationToken cancellationToken = default
            )
        {
            if (filePath.IsNullOrWhiteSpace())
                throw new System.NotSupportedException($"Save game loading without {nameof(filePath)} not supported");

            await SavingSystem.Self.LoadFromFileAsync(filePath, cancellationToken);
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

            gameplaySession.Dispose();

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

        public Observable<int> ObserveGameplaySession()
        {
            return gameplaySession;
        }

        public Observable<bool> ObserveIsGameSaving()
        {
            return SavingSystem.Self.ObserveSaveLoadingStarted().Merge(SavingSystem.Self.ObserveSaveLoadingFinished());
        }

        public Observable<bool> ObserveIsSaveGameLoading()
        {
            return SavingSystem.Self.ObserveSavingStarted().Merge(SavingSystem.Self.ObserveSavingFinished());
        }
    }
}
