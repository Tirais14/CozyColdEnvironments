#nullable enable
using System;

namespace CCEnvs
{
    public interface ILoadable
    {
        event Action OnStartLoading;
        event Action OnLoaded;

        bool IsLoading { get; }
        bool IsLoaded { get; }
    }
}
