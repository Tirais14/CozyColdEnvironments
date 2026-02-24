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

        protected override void Init()
        {
            base.Init();
            ResetRecordViews();
            SetRecordViews();
            BindScoreRecordAdd();
            BindScoreRecordRemove();
            BindScoreRecordsClear();
            BindScoreRecordReplaced();
            BindProfileIconView();
            BindPositionView();
            SetProfileNameView();
        }

        protected override Maybe<LeaderboardEntryViewModel> CreateViewModel()
        {
            return new LeaderboardEntryViewModel(LeaderboardEntry.Empty, destroyCancellationToken);
        }

        private void UpdateScoreValueView(string key, string scoreView)
        {
            scoreRecrodViews.Deserialized[key].text = scoreView;
        }

        private void SetRecordViews()
        {
            foreach (var item in viewModelUnsafe.ScoreRecords.Unfiltered)
            {
                if (!scoreRecrodViews.Deserialized.ContainsKey(item.Value.Key))
                    continue;

                UpdateScoreValueView(item.Value.Key, item.View);
            }
        }

        private void ResetRecordViews()
        {
            foreach (var item in viewModelUnsafe.ScoreRecords.Unfiltered)
            {
                if (!scoreRecrodViews.Deserialized.ContainsKey(item.Value.Key))
                    continue;

                UpdateScoreValueView(item.Value.Key, UNBINDED_SCORE_RECORD_VALUE);
            }
        }

        private void BindScoreRecordAdd()
        {
            viewModelUnsafe.ScoreRecords.ObserveAdd(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) =>
                {
                    @this.UpdateScoreValueView(item.Value.Key, item.View);
                })
                .AddTo(viewModelDisposables);
        }

        private void BindScoreRecordRemove()
        {
            viewModelUnsafe.ScoreRecords.ObserveRemove(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) =>
                {
                    @this.UpdateScoreValueView(item.Value.Key, UNBINDED_SCORE_RECORD_VALUE);
                })
                .AddTo(viewModelDisposables);
        }

        private void BindScoreRecordsClear()
        {
            viewModelUnsafe.ScoreRecords.ObserveClear(destroyCancellationToken)
                .Subscribe(this,
                static (_, @this) =>
                {
                    foreach (var scoreView in @this.scoreRecrodViews.Deserialized)
                        scoreView.Value.text = UNBINDED_SCORE_RECORD_VALUE;
                })
                .AddTo(viewModelDisposables);
        }

        private void BindScoreRecordReplaced()
        {
            viewModelUnsafe.ScoreRecords.ObserveReplace(destroyCancellationToken)
                .Subscribe(this,
                static (item, @this) =>
                {
                    @this.UpdateScoreValueView(item.NewValue.Value.Key, item.NewValue.View);
                })
                .AddTo(viewModelDisposables);
        }

        private void BindProfileIconView()
        {
            if (profileIconView == null)
                return;

            viewModelUnsafe.ProfileIcon.Subscribe(this,
                static (icon, @this) =>
                {
                    @this.profileIconView!.sprite = icon;
                })
                .AddTo(viewModelDisposables);
        }

        private void BindPositionView()
        {
            if (positionView == null)
                return;

            viewModelUnsafe.Position.Subscribe(this,
                static (pos, @this) =>
                {
                    @this.positionView!.text = pos;
                })
                .AddTo(viewModelDisposables);
        }

        private void SetProfileNameView()
        {
            if (profileNameView == null)
                return;

            profileNameView.text = viewModelUnsafe.ProfileName;
        }
    }
}
