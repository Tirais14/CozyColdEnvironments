using R3;
using YG;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public sealed class YandexAdvertisementAPI : IAdvertisementAPI
    {
        public static YandexAdvertisementAPI? Instance { get; private set; }

        private readonly ReactiveProperty<int> advertisementCount = new();

        public bool IsAdvertisementShown => advertisementCount.Value > 0;
        public int AdvertisementCount => advertisementCount.Value;

        public YandexAdvertisementAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexAdvertisementAPI));

            YG2.onCloseAnyAdv += () =>
            {
                advertisementCount.Value--;
            };

            YG2.onOpenAnyAdv += () =>
            {
                advertisementCount.Value++;
            };

            Instance = this;
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
            return advertisementCount.Select(
                static count =>
                {
                    return count > 0;
                });
        }

        public Observable<int> ObserveAdvertisementCount()
        {
            return advertisementCount;
        }
    }
}
