#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public class ExternalAPIProvider : IExternalAPIProvider
    {
        public IGeneralAPI GeneralAPI { get; }

        public IPlayerAPI? PlayerAPI { get; }

        public IAdvertisementAPI? AdvertisementAPI { get; }

        public ISavingAPI? SavingAPI { get; }

        public ILocalizationAPI? LocalizationAPI { get; }

        public ILeaderboardAPI? LeaderboardAPI { get; }

        public ExternalAPIProvider(
            IGeneralAPI generalAPI,
            IPlayerAPI? playerAPI = null,
            IAdvertisementAPI? advertisementAPI = null,
            ISavingAPI? savingAPI = null,
            ILocalizationAPI? localizationAPI = null,
            ILeaderboardAPI? leaderboardAPI = null)
        {
            CC.Guard.IsNotNull(generalAPI, nameof(generalAPI));

            GeneralAPI = generalAPI;

            PlayerAPI = playerAPI;
            AdvertisementAPI = advertisementAPI;
            SavingAPI = savingAPI;
            LocalizationAPI = localizationAPI;
            LeaderboardAPI = leaderboardAPI;
        }
    }
}
