using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public interface IAdvertisementAPI : IDisposable
    {
        bool IsAdvertisementShown { get; }
        int AdvertisementCount { get; }

        Observable<bool> ObserveIsAdvertisementShown();
        Observable<int> ObserveAdvertisementCount();
    }
}
