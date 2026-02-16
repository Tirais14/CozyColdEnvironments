using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    public struct AdvertisementInfo
    {
        public float ShowInterval { get; private set; }

        public DateTimeOffset LastShownTime { get; private set; }

        public TimeProvider TimeProvider { get; init; }

        public AdvertisementInfo SetShowInterval(float internval)
        {
            Guard.IsGreaterThanOrEqualTo(internval, 0, nameof(internval));

            ShowInterval = internval;

            return this;
        }

        public AdvertisementInfo SetShown()
        {
            LastShownTime = TimeProvider.GetUtcNow();

            return this;
        }

        public readonly bool IsCanShow()
        {
            if (LastShownTime == default)
                return true;

            return (LastShownTime - TimeProvider.GetUtcNow()).TotalSecondsF() > ShowInterval;
        }

        public bool TrySetShown()
        {
            if (!IsCanShow())
                return false;

            SetShown();
            return true;
        }

        public readonly override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(ShowInterval)}: {ShowInterval})";
        }
    }
}
