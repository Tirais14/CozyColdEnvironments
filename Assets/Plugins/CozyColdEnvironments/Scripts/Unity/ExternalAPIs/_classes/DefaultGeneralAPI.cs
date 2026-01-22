//using CCEnvs.FuncLanguage;
//using CCEnvs.Unity.ExternalAPIs;
//using CCEnvs.Unity.ExternalAPIs.Yandex;
//using CCEnvs.Unity.Saves;
//using Cysharp.Threading.Tasks;
//using R3;
//using System.Threading;

//#nullable enable
//namespace CCEnvs.Unity
//{
//    public class DefaultGeneralAPI : IGeneralAPI
//    {
//        private readonly ReactiveProperty<bool> isInitialized = new();
//        private readonly ReactiveProperty<bool> isGameReady = new();
//        private readonly ReactiveProperty<bool> isGameplayMode = new();
//        private readonly ReactiveProperty<bool> isGamePaused = new();
//        private readonly ReactiveProperty<bool> isGameWindowShown = new();
//        private readonly ReactiveProperty<bool> isGameWindowFocused = new();
//        private readonly ReactiveProperty<bool> isGameSaving = new();

//        public Maybe<IPlayerAPI> PlayerAPI => throw new System.NotImplementedException();
//        public Maybe<IAdvertisementAPI> AdvertisementAPI => throw new System.NotImplementedException();

//        public bool IsInitialized => isInitialized.Value;
//        public bool IsGameReady => isGameReady.Value;
//        public bool IsGameplayMode => isGameplayMode.Value;
//        public bool IsGamePaused => isGamePaused.Value;
//        public bool IsGameWindowShown => isGameWindowShown.Value;
//        public bool IsGameWindowFocused => isGameWindowFocused.Value;
//        public bool IsGameSaving => isGameSaving.Value;

//        public void GameplayStart()
//        {
//            isGameplayMode.Value = true;
//        }

//        public void GameplayStop()
//        {
//            isGameplayMode.Value = false;
//        }

//        public void Initialize()
//        {
//            isInitialized.Value = true;
//        }

//        public void PauseGame()
//        {
//            isGamePaused.Value = true;
//        }

//        public void UnpauseGame()
//        {
//            isGamePaused.Value = false;
//        }

//        public UniTask SaveGame()
//        {
//            return UniTask.CompletedTask;
//        }

//        public UniTask LoadSaveGame()
//        {
//            return UniTask.CompletedTask;
//        }

//        //public async UniTask SaveGameAsync(
//        //    string? path = null,
//        //    string? serializedData = null, 
//        //    CancellationToken cancellationToken = default
//        //    )
//        //{
//        //    if (path.IsNullOrWhiteSpace())
//        //        await SavingSystem.Self.SaveInMemoryAsync(cancellationToken);

//        //    await SavingSystem.Self.SaveInMemoryAsync(cancellationToken);
//        //}

//        //public async UniTask<string> LoadGameAsync(
//        //    string? path = null,
//        //    CancellationToken cancellationToken = default
//        //    )
//        //{
//        //    if (path.IsNullOrWhiteSpace())
//        //        return SavingSystem.Self.LoadedFileDataRaw ?? string.Empty;

//        //    return await SavingSystem.Self.LoadFromFileAsync(path, cancellationToken);
//        //}

//        public void SetGameReady(bool state)
//        {
//            isGameReady.Value = state;
//        }

//        private bool disposed;
//        public void Dispose()
//        {
//            if (disposed)
//                return;

//            PlayerAPI.IfSome(api => api.Dispose());
//            AdvertisementAPI.IfSome(api => api.Dispose());

//            isInitialized.Dispose();
//            isGameReady.Dispose();
//            isGameplayMode.Dispose();
//            isGamePaused.Dispose();
//            isGameWindowShown.Dispose();
//            isGameWindowFocused.Dispose();

//            disposed = true;
//        }

//        public Observable<bool> ObserveIsInitialized()
//        {
//            return isInitialized;
//        }

//        public Observable<bool> ObserveIsGamePaused()
//        {
//            return isGamePaused;
//        }

//        public Observable<bool> ObserveIsGameplayMode()
//        {
//            return isGameplayMode;
//        }

//        public Observable<bool> ObserveIsGameReady()
//        {
//            return isGameReady;
//        }

//        public Observable<bool> ObserveIsGameSaving()
//        {
//            return isGameSaving;
//        }

//        public Observable<bool> ObserveIsGameWindowFocused()
//        {
//            return isGameWindowFocused;
//        }

//        public Observable<bool> ObserveIsGameWindowShown()
//        {
//            return isGameWindowShown;
//        }
//    }
//}
