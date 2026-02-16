using System;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    [Flags]
    public enum AdvertisementTypes
    {
        None,
        Fullscreen = 1,
        Rewarding = 1 << 1,
        Banner = 1 << 2,
        Sticker = 1 << 3,
        Other = 1 << 4,
        Any = 1 << 5,
    }
}
