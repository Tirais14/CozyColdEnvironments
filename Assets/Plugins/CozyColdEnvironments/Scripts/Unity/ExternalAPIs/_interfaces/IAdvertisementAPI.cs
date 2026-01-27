using R3;
using System;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public interface IAdvertisementAPI : IDisposable
    {
        bool IsAdvertisementShown { get; }
        int AdvertisementCount { get; }

        void ShowAdvertisement(AdvertisementTypes advertisementType, object? key = null);

        Observable<bool> ObserveIsAdvertisementShown();

        Observable<int> ObserveAdvertisementCount();

        Observable<AdvertisementTypes> ObserveShownAdvertisementType();

        Observable<AdvertisementTypes> ObserveShowAdvertisementError();
    }
}
