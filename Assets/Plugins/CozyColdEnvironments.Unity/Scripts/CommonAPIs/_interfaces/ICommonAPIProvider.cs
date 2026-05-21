#nullable enable
namespace CCEnvs.UnityX.CommonAPIs
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
