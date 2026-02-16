#if PLUGIN_YG_2 && Localization_yg && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Dependencies;
using CCEnvs.Patterns.Commands;
using Cysharp.Threading.Tasks;
using R3;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using YG;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.Yandex
{
    public class YandexLocalizationAPI : ILocalizationAPI
    {
        [field: OnInstallResetable]
        public static YandexLocalizationAPI? Instance { get; private set; }

        private readonly CancellationTokenSource cancellationTokenSource = new();

        private readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update);

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private Observable<string>? selectedLocaleObs;

        public string SelectedLocale => YG2.lang;

        public YandexLocalizationAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexLocalizationAPI));

            BindToLocaleChangedEvent();
            BindLanguageSwitched();

            Instance = this;
            BuiltInDependecyContainer.BindTo<ILocalizationAPI>(this);
            BuiltInDependecyContainer.BindTo(this);
        }

        public void SetLocale(string code)
        {
            YG2.SwitchLanguage(code);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();

            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            YG2.onSwitchLang -= OnLanguageSwitched;

            disposed = true;
        }

        public Observable<string> ObserveSelectedLocale()
        {
            selectedLocaleObs ??= Observable.EveryValueChanged(default(object)!,
                static _ =>
                {
                    return YG2.lang;
                },
                cancellationToken: cancellationTokenSource.Token
                );

            return selectedLocaleObs;
        }

        private void OnLocaleChanged(Locale locale)
        {
            var codeMatch = Regex.Match(locale.LocaleName, @"\((\w*)\)\s*");

            if (!codeMatch.Success || codeMatch.Groups.IsEmpty())
                throw new System.InvalidOperationException($"Cannot parse locale name: {locale.LocaleName}");

            SetLocale(codeMatch.Groups[1].Value);
        }

        private void OnLanguageSwitched(string code)
        {
            Command.Builder.SetName(nameof(OnLanguageSwitched), this)
                .SetSingle()
                .WithState((@this: this, code))
                .Asyncronously()
                .SetExecuteAction(
                static async (args, cancellationToken) =>
                {
                    await LocalizationSettings.InitializationOperation.Task;

                    var locale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(args.code));

                    LocalizationSettings.SelectedLocale = locale;
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(disposeCancellationTokenSource.Token)
                .ScheduleBy(commandScheduler);
        }

        private void BindToLocaleChangedEvent()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        private void BindLanguageSwitched()
        {
            YG2.onSwitchLang += OnLanguageSwitched;
        }
    }
}
#endif
