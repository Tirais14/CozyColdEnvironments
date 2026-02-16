#nullable enable
using CCEnvs.Attributes;
using CCEnvs.Dependencies;

namespace CCEnvs.Unity.CommonAPIs
{
    public class CommonAPIProvider : ICommonAPIProvider
    {
        [field: OnInstallResetable]
        public CommonAPIProvider? Instance { get; private set; }

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
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(CommonAPIProvider));

            CC.Guard.IsNotNull(generalAPI, nameof(generalAPI));

            GeneralAPI = generalAPI;

            PlayerAPI = playerAPI;
            AdvertisementAPI = advertisementAPI;
            SavingAPI = savingAPI;
            LocalizationAPI = localizationAPI;
            LeaderboardAPI = leaderboardAPI;

            Instance = this;

            BuiltInDependecyContainer.BindTo<ICommonAPIProvider>(this);
            BuiltInDependecyContainer.BindTo(this);
        }
    }
}
