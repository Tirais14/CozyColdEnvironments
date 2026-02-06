using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Leaderboards;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Linq;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardView : View<LeaderboardViewModel>
    {
        protected override void Init()
        {
            base.Init();
            BindEntryClear();
            BindEntryRemove();
            BindEntryAdd();
            BindSortedEntries();
        }

        protected override Maybe<LeaderboardViewModel> CreateViewModel()
        {
            return this.Q()
                .IncludeInactive()
                .Component<LeaderboardViewModel>()
                .Lax()
                .IfSome(static vm => vm.SetModel(new Leaderboard()));
        }

        private void BindEntryAdd()
        {
            viewModelUnsafe.Entries.ObserveDictionaryAdd(viewModelUnsafe.DisposeCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) =>
                {
                    @this.viewModelUnsafe.OnEntryAdd(entry);

                })
                .AddTo(viewModelDisposables);
        }

        private void BindEntryRemove()
        {
            viewModelUnsafe.Entries.ObserveDictionaryRemove(viewModelUnsafe.DisposeCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (entry, @this) =>
                {
                    @this.viewModelUnsafe.OnEntryRemove(entry);
                })
                .AddTo(viewModelDisposables);
        }

        private void BindEntryClear()
        {
            viewModelUnsafe.Entries.ObserveClear(viewModelUnsafe.DisposeCancellationToken)
                .Subscribe(this,
                static (_, @this) => @this.viewModelUnsafe.OnEntriesClear())
                .AddTo(viewModelDisposables);
        }

        private void BindSortedEntries()
        {
            viewModelUnsafe.SortedEntries.ObserveChanged(viewModelUnsafe.DisposeCancellationToken)
                .ChunkFrame(1)
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.viewModelUnsafe.SortEntries();
                })
                .AddTo(viewModelDisposables);
        }
    }
}
