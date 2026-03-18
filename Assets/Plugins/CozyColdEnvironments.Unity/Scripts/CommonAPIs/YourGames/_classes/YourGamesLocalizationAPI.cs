#if YOUR_GAMES_PLUGIN_ENABLED && PLUGIN_YG_2 && Localization_yg && PLATFORM_WEBGL
using CCEnvs.Attributes;
using CCEnvs.Collections;
using CCEnvs.Dependencies;
using CCEnvs.Patterns.Commands;
using R3;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using YG;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs.YourGames
{
    public class YourGamesLocalizationAPI : ILocalizationAPI
    {
        [field: OnInstallResetable]
        public static YourGamesLocalizationAPI? Instance { get; private set; }

        private readonly CancellationTokenSource cancellationTokenSource = new();

        private readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update);

        private readonly CancellationTokenSource disposeCancellationTokenSource = new();

        private Observable<string>? selectedLocaleObs;

        public string SelectedLocale => YG2.lang;

        public YourGamesLocalizationAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YourGamesLocalizationAPI));

            BindToLocaleChangedEvent();
            BindLanguageSwitched();

            Instance = this;

            CCServices.Bind<ILocalizationAPI>(this);
            CCServices.Bind(this);
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

            CCServices.Unbind<ILocalizationAPI>();
            CCServices.Unbind(GetType());

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
            Command.Builder.WithName(nameof(OnLanguageSwitched), this)
                .AsSingle()
                .WithState((@this: this, code))
                .Asynchronously()
                .WithExecuteAction(
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
