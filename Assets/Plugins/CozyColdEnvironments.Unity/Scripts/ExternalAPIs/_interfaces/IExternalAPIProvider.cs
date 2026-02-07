using CCEnvs.FuncLanguage;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public interface IExternalAPIProvider
    {
        IGeneralAPI GeneralAPI { get; }
        IPlayerAPI? PlayerAPI { get; }
        IAdvertisementAPI? AdvertisementAPI { get; }
        ISavingAPI? SavingAPI { get; }
        ILocalizationAPI? LocalizationAPI { get; }
        ILeaderboardAPI? LeaderboardAPI { get; }
    }
}
