using CCEnvs.FuncLanguage;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public interface IExternalAPIProvider
    {
        IGeneralAPI GeneralAPI { get; }
        Maybe<IPlayerAPI> PlayerAPI { get; }
        Maybe<IAdvertisementAPI> AdvertisementAPI { get; }
        Maybe<ISavingAPI> SavingAPI { get; }
        Maybe<ILocalizationAPI> LocalizationAPI { get; }
    }
}
