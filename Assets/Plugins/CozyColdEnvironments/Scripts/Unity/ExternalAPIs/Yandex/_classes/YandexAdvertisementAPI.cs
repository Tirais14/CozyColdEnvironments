#if YandexGamesPlatform_yg && PLATFORM_WEBGL
using CCEnvs.Attributes;
using R3;
using UnityEditor;
using YG;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public sealed class YandexAdvertisementAPI : IAdvertisementAPI
    {
        [OnInstallResetable]
        public static YandexAdvertisementAPI? Instance { get; private set; }

        private readonly ReactiveProperty<int> advertisementCount = new();
        private readonly ReactiveProperty<AdvertisementTypes> shownAdvertisementType = new();

        public bool IsAdvertisementShown => advertisementCount.Value > 0;
        public int AdvertisementCount => advertisementCount.Value;
        public AdvertisementTypes ShownAdvertisementType => shownAdvertisementType.Value;

        public YandexAdvertisementAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexAdvertisementAPI));

            YG2.onCloseAnyAdv += () =>
            {
                advertisementCount.Value--;

                if (advertisementCount.Value < 0)
                    advertisementCount.Value = 0;

                if (advertisementCount.Value == 0)
                    shownAdvertisementType.Value = AdvertisementTypes.None;
            };

            YG2.onOpenAnyAdv += () =>
            {
                advertisementCount.Value++;

                if (shownAdvertisementType.Value == AdvertisementTypes.None)
                    shownAdvertisementType.Value = AdvertisementTypes.Any;
            };

            Instance = this;
        }

        public void ShowAdvertisement(AdvertisementTypes advertisementType, object? key = null)
        {
            if (advertisementType == AdvertisementTypes.None)
                throw new System.ArgumentException(advertisementType.ToString(), nameof(advertisementType));

            if (advertisementType.IsFlagSetted(AdvertisementTypes.Banner))
            {
                YG2.ShowBanner();
                shownAdvertisementType.Value = AdvertisementTypes.Banner;
            }
            else if (advertisementType.IsFlagSetted(AdvertisementTypes.Fullscreen))
                throw new System.NotImplementedException();
            else if (advertisementType.IsFlagSetted(AdvertisementTypes.Rewarding))
                throw new System.NotImplementedException();
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            advertisementCount.Dispose();

            disposed = true;
        }

        public Observable<bool> ObserveIsAdvertisementShown()
        {
            return advertisementCount.Pairwise()
                .Where(
                static pair =>
                {
                    return pair.Current != pair.Previous;
                })
                .Select(
                static pair =>
                {
                    return pair.Current > 0;
                });
        }

        public Observable<int> ObserveAdvertisementCount()
        {
            return advertisementCount;
        }

        public Observable<AdvertisementTypes> ObserveShownAdvertisementType()
        {
            return shownAdvertisementType;
        }
    }
}
#endif
