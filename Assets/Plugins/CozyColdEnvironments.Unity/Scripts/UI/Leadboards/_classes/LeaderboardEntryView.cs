#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Leaderboards;
using ObservableCollections;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CCEnvs.Unity.UI.Leaderboards
{
    public class LeaderboardEntryView : View<LeaderboardEntryViewModel>
    {
        public const string UNBINDED_SCORE_RECORD_VALUE = "unbinded";

        [SerializeField]
        [FormerlySerializedAs("scoreViews")]
        protected SerializedDictionary<string, TMP_Text> scoreRecrodViews = new();

        [SerializeField]
        protected Image? profileIconView;

        [SerializeField]
        protected TMP_Text? profileNameView;

        [SerializeField]
        protected TMP_Text? positionView;

        protected override void InitViewModel(LeaderboardEntryViewModel vm)
        {
            ResetRecordViews(vm);
            SetRecordViews(vm);
            BindScoreRecordAdd(vm);
            BindScoreRecordRemove(vm);
            BindScoreRecordsClear(vm);
            BindScoreRecordReplaced(vm);
            BindProfileIconView(vm);
            BindPositionView(vm);
            SetProfileNameView(vm);
        }

        protected override LeaderboardEntryViewModel? CreateViewModel()
        {
            return new LeaderboardEntryViewModel(LeaderboardEntry.Empty);
        }

        private void UpdateScoreValueView(string key, string scoreView)
        {
            scoreRecrodViews.Deserialized[key].text = scoreView;
        }

        private void SetRecordViews(LeaderboardEntryViewModel vm)
        {
            foreach (var item in vm.ScoreRecords.Unfiltered)
            {
                if (!scoreRecrodViews.Deserialized.ContainsKey(item.Value.Key))
                    continue;

                UpdateScoreValueView(item.Value.Key, item.View);
            }
        }

        private void ResetRecordViews(LeaderboardEntryViewModel vm)
        {
            foreach (var item in vm.ScoreRecords.Unfiltered)
            {
                if (!scoreRecrodViews.Deserialized.ContainsKey(item.Value.Key))
                    continue;

                UpdateScoreValueView(item.Value.Key, UNBINDED_SCORE_RECORD_VALUE);
            }
        }

        private void BindScoreRecordAdd(LeaderboardEntryViewModel vm)
        {
            vm.ScoreRecords.ObserveAdd(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) =>
                {
                    @this.UpdateScoreValueView(item.Value.Key, item.View);
                })
                .AddTo(ViewModelDisposables);
        }

        private void BindScoreRecordRemove(LeaderboardEntryViewModel vm)
        {
            vm.ScoreRecords.ObserveRemove(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) =>
                {
                    @this.UpdateScoreValueView(item.Value.Key, UNBINDED_SCORE_RECORD_VALUE);
                })
                .AddTo(ViewModelDisposables);
        }

        private void BindScoreRecordsClear(LeaderboardEntryViewModel vm)
        {
            vm.ScoreRecords.ObserveClear(destroyCancellationToken)
                .Subscribe(this,
                static (_, @this) =>
                {
                    foreach (var scoreView in @this.scoreRecrodViews.Deserialized)
                        scoreView.Value.text = UNBINDED_SCORE_RECORD_VALUE;
                })
                .AddTo(ViewModelDisposables);
        }

        private void BindScoreRecordReplaced(LeaderboardEntryViewModel vm)
        {
            vm.ScoreRecords.ObserveReplace(destroyCancellationToken)
                .Subscribe(this,
                static (item, @this) =>
                {
                    @this.UpdateScoreValueView(item.NewValue.Value.Key, item.NewValue.View);
                })
                .AddTo(ViewModelDisposables);
        }

        private void BindProfileIconView(LeaderboardEntryViewModel vm)
        {
            if (profileIconView == null)
                return;

            vm.ProfileIcon.Subscribe(this,
                static (icon, @this) =>
                {
                    @this.profileIconView!.sprite = icon;
                })
                .AddTo(ViewModelDisposables);
        }

        private void BindPositionView(LeaderboardEntryViewModel vm)
        {
            if (positionView == null)
                return;

            vm.Position.Subscribe(this,
                static (pos, @this) =>
                {
                    @this.positionView!.text = pos;
                })
                .AddTo(ViewModelDisposables);
        }

        private void SetProfileNameView(LeaderboardEntryViewModel vm)
        {
            if (profileNameView == null)
                return;

            profileNameView.text = vm.ProfileName;
        }
    }
}
