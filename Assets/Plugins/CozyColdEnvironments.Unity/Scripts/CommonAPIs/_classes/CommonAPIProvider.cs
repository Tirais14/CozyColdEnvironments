#nullable enable
using CCEnvs.Dependencies;

namespace CCEnvs.Unity.CommonAPIs
{
    public class CommonAPIProvider : ICommonAPIProvider
    {
        public IGeneralAPI GeneralAPI { get; }

        public IPlayerAPI? PlayerAPI { get; }

        public IAdvertisementAPI? AdvertisementAPI { get; }

        public ISavingAPI? SavingAPI { get; }

        public ILocalizationAPI? LocalizationAPI { get; }

        public ILeaderboardAPI? LeaderboardAPI { get; }

        public CommonAPIProvider(
            IGeneralAPI generalAPI,
            IPlayerAPI? playerAPI = null,
            IAdvertisementAPI? advertisementAPI = null,
            ISavingAPI? savingAPI = null,
            ILocalizationAPI? localizationAPI = null,
            ILeaderboardAPI? leaderboardAPI = null
            )
        {
            CC.Guard.IsNotNull(generalAPI, nameof(generalAPI));

            GeneralAPI = generalAPI;

            PlayerAPI = playerAPI;
            AdvertisementAPI = advertisementAPI;
            SavingAPI = savingAPI;
            LocalizationAPI = localizationAPI;
            LeaderboardAPI = leaderboardAPI;
        }

        public static CommonAPIProvider CreateFromBuiltInDependecyContainer()
        {
            return new CommonAPIProvider(
                CCServices.Resolve<IGeneralAPI>(),
                CCServices.TryResolve<IPlayerAPI>(),
                CCServices.TryResolve<IAdvertisementAPI>(),
                CCServices.TryResolve<ISavingAPI>(),
                CCServices.TryResolve<ILocalizationAPI>(),
                CCServices.TryResolve<ILeaderboardAPI>()
                );
        }
    }
}
