#nullable enable
using ObservableCollections;
using R3;
using System.Collections.Generic;

namespace CCEnvs.Unity.UI.Leaderboards
{
    public interface ILeaderboardEntryViewModel : IViewModel
    {
        ISynchronizedView<KeyValuePair<string, ReactiveProperty<float>>, Observable<string>> Values { get; }

        ReadOnlyReactiveProperty<float> Score { get; }
    }
}
