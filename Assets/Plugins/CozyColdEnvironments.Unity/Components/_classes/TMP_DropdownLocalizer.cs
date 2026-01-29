using CCEnvs.Patterns.Commands;
using CCEnvs.Unity.Injections;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using OptionData = TMPro.TMP_Dropdown.OptionData;
using OptionDataLocalized = CCEnvs.Unity.TMP_DropdownOptionDataLocalized;

#nullable enable
namespace CCEnvs.Unity.Components
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class TMP_DropdownLocalizer : CCBehaviour
    {
        private readonly CommandScheduler commandScheduler = new(
            UnityFrameProvider.Update,
            nameof(TMP_DropdownLocalizer)
            );

        [SerializeField]
        private LocalizedStringTable localizedStringTable = null!;

        [GetBySelf]
        private TMP_Dropdown dropdown = null!;

        private Action<Locale> onLocaleChanged = null!;

        protected override void Awake()
        {
            base.Awake();

            onLocaleChanged = _ => OnLocaleChangedAsync().Forget();

            LocalizationSettings.SelectedLocaleChanged += onLocaleChanged;
        }

        protected override void Start()
        {
            base.Start();
            OnLocaleChangedAsync().Forget();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            commandScheduler.Dispose();
            LocalizationSettings.SelectedLocaleChanged -= onLocaleChanged;
        }

        private async UniTaskVoid OnLocaleChangedAsync()
        {
            Command.Builder.NextStep(this)
                .Asyncronously()
                .SetExecuteAction(async (@this, token) =>
                {
                    var tokenSource = token.LinkTokens(@this.destroyCancellationToken);

                    try
                    {
                        await @this.RecreateOptionsAsync(tokenSource.Token);

                        await UpdateDropdownValueAsync();
                    }
                    finally
                    {
                        tokenSource.Dispose();
                    }
                })
                .BuildPooled()
                .Value
                .ScheduleBy(commandScheduler)
                .AttachExternalCancellationToken(destroyCancellationToken);
        }

        private void TryApplyLocalizationToOption(OptionData option, StringTable table, string key)
        {
            var localizedString = table[key];

            if (localizedString is null)
            {
                table.PrintError($"Key {key} not found");
                return;
            }

            option.text = localizedString.GetLocalizedString();
        }

        private async UniTask UpdateDropdownValueAsync()
        {
            if (dropdown.options.Count <= 1)
                return;

            await UniTask.Yield(timing: PlayerLoopTiming.PostLateUpdate);

            var dropdownValue = dropdown.value;
            var optionCount = dropdown.options.Count;

            for (int i = 0; i < optionCount; i++)
            {
                if (i == dropdownValue)
                    continue;

                dropdown.value = i;

                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

                dropdown.value = dropdownValue;

                break;
            }
        }

        private async UniTask RecreateOptionsAsync(CancellationToken cancellationToken)
        {
            var table = await localizedStringTable.GetTableAsync().Task;

            var options = dropdown.options;

            OptionData option;

            int optionsCount = options.Count;

            for (int i = 0; i < optionsCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequestedByInterval(i);

                option = options[i];

                if (option is not OptionDataLocalized optionLoc)
                    optionLoc = new OptionDataLocalized(option, option.text);

                options[i] = optionLoc;

                TryApplyLocalizationToOption(optionLoc, table, optionLoc.LocalizedStringKey);
            }
        }
    }
}
