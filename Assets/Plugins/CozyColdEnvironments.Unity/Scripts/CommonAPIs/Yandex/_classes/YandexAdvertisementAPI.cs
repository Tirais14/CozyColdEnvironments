#if PLUGIN_YG_2 && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Dependencies;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;
using UnityEditor;
using YG;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    public sealed class YandexAdvertisementAPI : IAdvertisementAPI
    {
        [field: OnInstallResetable]
        public static YandexAdvertisementAPI? Instance { get; private set; }

        private readonly ReactiveProperty<int> advertisementCount = new();

        private readonly Dictionary<AdvertisementTypes, AdvertisementInfo> advertisementInfos = new();

        private ReactiveCommand<AdvertisementTypes>? shownAdvertisementTypeCmd;
        private ReactiveCommand<AdvertisementTypes>? showAdvertisementErrorCmd;

        public bool IsAdvertisementShown => advertisementCount.Value > 0;
        public int AdvertisementCount => advertisementCount.Value;

        public TimeProvider TimeProvider { get; }

        public YandexAdvertisementAPI(TimeProvider? timeProvider = null)
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexAdvertisementAPI));

            TimeProvider = timeProvider ?? UnityTimeProvider.Update;

            BindEvents();
            SetupAdvertisementInfos();

            Instance = this;

            BuiltInDependecyContainer.Bind<IAdvertisementAPI>(this);
            BuiltInDependecyContainer.Bind(this);
        }

        public void ShowAdvertisement(AdvertisementTypes advertisementType, object? key = null)
        {
            if (advertisementType.IsFlagSetted(AdvertisementTypes.Banner))
            {
#if BannerAdv_yg
                throw new NotImplementedException();
#endif //BannerAdv_yg
            }
            else if (advertisementType.IsFlagSetted(AdvertisementTypes.Fullscreen))
            {
#if InterstitialAdv_yg
                if (!YG2.isTimerAdvCompleted
                    ||
                    !advertisementInfos.TryGetValue(advertisementType, out var adInfo)
                    ||
                    !adInfo.TrySetShown())
                {
                    return;
                }

                YG2.InterstitialAdvShow();

                return;
#endif //InterstitialAdv_yg
            }
            else if (advertisementType.IsFlagSetted(AdvertisementTypes.Rewarding))
                throw new NotSupportedException(AdvertisementTypes.Rewarding.ToString());

            throw new NotSupportedException(advertisementType.ToString());
        }

        public IAdvertisementAPI SetAdvertisementShowInterval(
            AdvertisementTypes advertisementType, 
            float intervalInSeconds
            )
        {
            if (advertisementType == AdvertisementTypes.None)
                throw new ArgumentException(advertisementType.ToString(), nameof(advertisementType));

            Guard.IsGreaterThanOrEqualTo(intervalInSeconds, 0, nameof(intervalInSeconds));

            advertisementInfos[advertisementType] = advertisementInfos.GetOrCreateNew(advertisementType)
                .SetShowInterval(intervalInSeconds);

            return this;
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

        private void BindEvents()
        {
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
        }

        private void SetupAdvertisementInfos()
        {
            var pairs = new KeyValuePair<AdvertisementTypes, AdvertisementInfo>[]
            {
                new(AdvertisementTypes.None, new AdvertisementInfo() { TimeProvider = TimeProvider }.SetShowInterval(float.PositiveInfinity)),
                new(AdvertisementTypes.Fullscreen, new AdvertisementInfo() { TimeProvider = TimeProvider }.SetShowInterval(0f)),
                new(AdvertisementTypes.Sticker, new AdvertisementInfo() { TimeProvider = TimeProvider }.SetShowInterval(0f)),
                new(AdvertisementTypes.Other, new AdvertisementInfo() { TimeProvider = TimeProvider }.SetShowInterval(0f)),
                new(AdvertisementTypes.Any, new AdvertisementInfo() { TimeProvider = TimeProvider }.SetShowInterval(0f)),
            };

            advertisementInfos.AddRange(pairs);
        }
    }
}
#endif
