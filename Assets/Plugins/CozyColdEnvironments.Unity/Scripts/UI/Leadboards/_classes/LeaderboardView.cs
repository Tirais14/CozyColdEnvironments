using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Leaderboards;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System.Linq;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardView : View<LeaderboardViewModel>
    {
        protected override void InitViewModel(LeaderboardViewModel vm)
        {
            base.InitViewModel(vm);

            BindEntryClear(vm);
            BindEntryRemove(vm);
            BindEntryAdd(vm);
            BindSortedEntries(vm);
        }

        protected override LeaderboardViewModel? CreateViewModel()
        {
            return this.Q()
                .IncludeInactive()
                .Component<LeaderboardViewModel>()
                .Lax()
                .IfSome(static vm => vm.SetModel(new Leaderboard()))
                .GetValue();
        }

        private void BindEntryAdd(LeaderboardViewModel vm)
        {
            vm.Entries.ObserveDictionaryAdd(vm.DisposeCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(vm.OnEntryAdd)
                .AddTo(ViewModelDisposables);
        }

        private void BindEntryRemove(LeaderboardViewModel vm)
        {
            vm.Entries.ObserveDictionaryRemove(vm.DisposeCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(vm.OnEntryRemove)
                .AddTo(ViewModelDisposables);
        }

        private void BindEntryClear(LeaderboardViewModel vm)
        {
            vm.Entries.ObserveClear(vm.DisposeCancellationToken)
                .Subscribe(vm,
                static (_, vm) => vm.OnEntriesClear())
                .AddTo(ViewModelDisposables);
        }

        private void BindSortedEntries(LeaderboardViewModel vm)
        {
            vm.SortedEntries.ObserveChanged(vm.DisposeCancellationToken)
                .ThrottleLastFrame(1)
                .Subscribe(vm,
                static (_, vm) =>
                {
                    vm.SortEntries();
                })
                .AddTo(ViewModelDisposables);
        }
    }
}
