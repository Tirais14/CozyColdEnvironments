#nullable enable
using CCEnvs.FuncLanguage;
using R3;
using System.Collections.Generic;
using System.Linq;

namespace CCEnvs.Unity.ExternalAPIs
{
    public class CompositeGeneralAPI : IGeneralAPI
    {
        private readonly List<IGeneralAPI> apis;

        public Maybe<IPlayerAPI> PlayerAPI { get; }
        public Maybe<IAdvertisementAPI> AdvertisementAPI { get; }

        public bool IsInitialized => apis.All(api => api.IsInitialized);
        public bool IsGameReady => apis.All(api => api.IsGameReady);
        public bool IsGameplayMode => apis.All(api => api.IsGameplayMode);
        public bool IsGamePaused => apis.All(api => api.IsGamePaused);
        public bool IsGameWindowShown => apis.All(api => api.IsGameWindowShown);
        public bool IsGameWindowFocused => apis.All(api => api.IsGameWindowFocused);

        public int GameplaySession => apis.Max(api => api.GameplaySession);

        public CompositeGeneralAPI(
            IPlayerAPI? playerAPI,
            IAdvertisementAPI? advertisementAPI,
            int capacity = 2)
        {
            PlayerAPI = playerAPI.Maybe();
            AdvertisementAPI = advertisementAPI.Maybe();

            apis = new List<IGeneralAPI>(capacity);
        }

        public CompositeGeneralAPI Add(IGeneralAPI api)
        {
            CC.Guard.IsNotNull(api, nameof(api));

            apis.Add(api);

            return this;
        }

        public void GameplayStart()
        {
            for (int i = 0; i < apis.Count; i++)
                apis[i].GameplayStart();
        }

        public void GameplayStop()
        {
            for (int i = 0; i < apis.Count; i++)
                apis[i].GameplayStop();
        }

        public void Initialize()
        {
            for (int i = 0; i < apis.Count; i++)
                apis[i].Initialize();
        }

        public void PauseGame()
        {
            for (int i = 0; i < apis.Count; i++)
                apis[i].PauseGame();
        }

        public void UnpauseGame()
        {
            for (int i = 0; i < apis.Count; i++)
                apis[i].UnpauseGame();
        }

        public void SetGameReady(bool state)
        {
            for (int i = 0; i < apis.Count; i++)
                apis[i].SetGameReady(state);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            apis.DisposeEach();

            disposed = true;
        }

        public Observable<bool> ObserveIsInitialized()
        {
            return ObservableHelper.MergeMany(apis, static api => api.ObserveIsInitialized());
        }

        public Observable<bool> ObserveIsGamePaused()
        {
            return ObservableHelper.MergeMany(apis, static api => api.ObserveIsGamePaused());
        }

        public Observable<bool> ObserveIsGameplayMode()
        {
            return ObservableHelper.MergeMany(apis, static api => api.ObserveIsGameplayMode());
        }

        public Observable<bool> ObserveIsGameReady()
        {
            return ObservableHelper.MergeMany(apis, static api => api.ObserveIsGameReady());
        }

        public Observable<bool> ObserveIsGameWindowFocused()
        {
            return ObservableHelper.MergeMany(apis, static api => api.ObserveIsGameWindowFocused());

        }

        public Observable<bool> ObserveIsGameWindowShown()
        {
            return ObservableHelper.MergeMany(apis, static api => api.ObserveIsGameWindowShown());

        }

        public Observable<int> ObserveGameplaySession()
        {
            return ObservableHelper.MergeMany(apis, static api => api.ObserveGameplaySession());
        }
    }
}
