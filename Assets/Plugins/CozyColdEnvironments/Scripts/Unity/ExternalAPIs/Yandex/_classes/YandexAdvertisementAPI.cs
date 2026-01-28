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
        [field: OnInstallResetable]
        public static YandexAdvertisementAPI? Instance { get; private set; }

        private readonly ReactiveProperty<int> advertisementCount = new();

        private ReactiveCommand<AdvertisementTypes>? shownAdvertisementTypeCmd;
        private ReactiveCommand<AdvertisementTypes>? showAdvertisementErrorCmd;

        public bool IsAdvertisementShown => advertisementCount.Value > 0;
        public int AdvertisementCount => advertisementCount.Value;

        public YandexAdvertisementAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexAdvertisementAPI));

            YG2.onOpenAnyAdv += () =>
            {
                advertisementCount.Value++;
                shownAdvertisementTypeCmd?.Execute(AdvertisementTypes.Any);
            };

            YG2.onCloseAnyAdv += () =>
            {
                advertisementCount.Value--;

                if (advertisementCount.Value < 0)
                    advertisementCount.Value = 0;
            };

            YG2.onErrorAnyAdv += () =>
            {
                showAdvertisementErrorCmd?.Execute(AdvertisementTypes.Any);
            };

#if BannerAdv_yg
            YG2.onBannerError += () =>
            {
                showAdvertisementErrorCmd?.Execute(AdvertisementTypes.Banner);
            };
#endif

#if InterstitialAdv_yg
            YG2.onOpenInterAdv += () =>
            {
                shownAdvertisementTypeCmd?.Execute(AdvertisementTypes.Fullscreen);
            };

            YG2.onErrorInterAdv += () =>
            {
                showAdvertisementErrorCmd?.Execute(AdvertisementTypes.Fullscreen);
            };
#endif

            Instance = this;
        }

        public void ShowAdvertisement(AdvertisementTypes advertisementType, object? key = null)
        {
            if (advertisementType == AdvertisementTypes.None)
                throw new System.ArgumentException(advertisementType.ToString(), nameof(advertisementType));

            if (advertisementType.IsFlagSetted(AdvertisementTypes.Banner))
            {
#if BannerAdv_yg
                YG2.ShowBanner();
#else
                throw new System.NotSupportedException(AdvertisementTypes.Banner.ToString());
#endif
            }
            else if (advertisementType.IsFlagSetted(AdvertisementTypes.Fullscreen))
            {
#if InterstitialAdv_yg
                YG2.InterstitialAdvShow();
#else
                throw new System.NotSupportedException(AdvertisementTypes.Fullscreen.ToString());
#endif
            }
            else if (advertisementType.IsFlagSetted(AdvertisementTypes.Rewarding))
                throw new System.NotSupportedException(AdvertisementTypes.Rewarding.ToString());
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            advertisementCount.Dispose();
            shownAdvertisementTypeCmd?.Dispose();
            showAdvertisementErrorCmd?.Dispose();

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
            shownAdvertisementTypeCmd ??= new ReactiveCommand<AdvertisementTypes>();

            return shownAdvertisementTypeCmd;
        }

        public Observable<AdvertisementTypes> ObserveShowAdvertisementError()
        {
            showAdvertisementErrorCmd ??= new ReactiveCommand<AdvertisementTypes>();

            return showAdvertisementErrorCmd;
        }
    }
}
#endif
