using R3;
using System;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    public interface IAdvertisementAPI : IDisposable
    {
        bool IsAdvertisementShown { get; }
        int AdvertisementCount { get; }

        TimeProvider TimeProvider { get; }

        void ShowAdvertisement(AdvertisementTypes advertisementType, object? key = null);

        IAdvertisementAPI SetAdvertisementShowInterval(
            AdvertisementTypes advertisementType,
            float intervalInSeconds
            );

        Observable<bool> ObserveIsAdvertisementShown();

        Observable<int> ObserveAdvertisementCount();

        Observable<AdvertisementTypes> ObserveShownAdvertisementType();

        Observable<AdvertisementTypes> ObserveShowAdvertisementError();
    }
}
