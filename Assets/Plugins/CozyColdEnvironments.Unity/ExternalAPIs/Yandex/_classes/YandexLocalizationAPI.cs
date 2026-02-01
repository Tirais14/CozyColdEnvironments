#if Localization_yg
using CCEnvs.Attributes;
using CCEnvs.Collections;
using R3;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using YG;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs.Yandex
{
    public class YandexLocalizationAPI : ILocalizationAPI
    {
        [field: OnInstallResetable]
        public static YandexLocalizationAPI? Instance { get; private set; }

        private readonly CancellationTokenSource cancellationTokenSource = new();

        private Observable<string>? selectedLocaleObs;

        public string SelectedLocale => YG2.lang;

        public YandexLocalizationAPI()
        {
            if (Instance is not null)
                throw CC.ThrowHelper.CannotCreateInstance(nameof(YandexLocalizationAPI));

            BindToLocaleChangedEvent();

            Instance = this;
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

        private void BindToLocaleChangedEvent()
        {
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }
    }
}
#endif
