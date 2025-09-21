#nullable enable
using System;

namespace CCEnvs
{
    public interface ILoadable
    {
        event Action<ILoadable> OnStartLoading;
        event Action<ILoadable> OnLoaded;

        bool IsLoaded { get; }
    }
}
