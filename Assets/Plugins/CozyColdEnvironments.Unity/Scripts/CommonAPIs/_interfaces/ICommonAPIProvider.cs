#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    public interface ICommonAPIProvider
    {
        IGeneralAPI GeneralAPI { get; }

        IPlayerAPI? PlayerAPI { get; }

        IAdvertisementAPI? AdvertisementAPI { get; }

        ISavingAPI? SavingAPI { get; }

        ILocalizationAPI? LocalizationAPI { get; }

        ILeaderboardAPI? LeaderboardAPI { get; }
    }
}
