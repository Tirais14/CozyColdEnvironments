using CCEnvs.FuncLanguage;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public class ExternalAPIProvider : IExternalAPIProvider
    {
        public IGeneralAPI GeneralAPI { get; }

        public Maybe<IPlayerAPI> PlayerAPI { get; }

        public Maybe<IAdvertisementAPI> AdvertisementAPI { get; }

        public Maybe<ISavingAPI> SavingAPI { get; }

        public Maybe<ILocalizationAPI> LocalizationAPI { get; }

        public ExternalAPIProvider(
            IGeneralAPI generalAPI,
            IPlayerAPI? playerAPI = null,
            IAdvertisementAPI? advertisementAPI = null,
            ISavingAPI? savingAPI = null,
            ILocalizationAPI? localizationAPI = null
            )
        {
            CC.Guard.IsNotNull(generalAPI, nameof(generalAPI));

            GeneralAPI = generalAPI;

            PlayerAPI = playerAPI.Maybe();
            AdvertisementAPI = advertisementAPI.Maybe();
            SavingAPI = savingAPI.Maybe();
            LocalizationAPI = localizationAPI.Maybe();
        }
    }
}
