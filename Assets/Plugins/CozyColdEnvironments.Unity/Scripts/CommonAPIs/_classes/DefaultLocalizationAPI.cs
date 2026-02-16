using R3;
using UnityEngine.Localization.Settings;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    //public sealed class DefaultLocalizationAPI : ILocalizationAPI
    //{
    //    private readonly ReactiveProperty<string> selectedLocale = new();

    //    public string SelectedLocale => selectedLocale.Value;

    //    public void SetLocale(string code)
    //    {
    //        selectedLocale.Value = code;

    //        var locale = LocalizationSettings.AvailableLocales.GetLocale(new UnityEngine.Localization.LocaleIdentifier(code));

    //        LocalizationSettings.SelectedLocale = locale;
    //    }

    //    private bool disposed;
    //    public void Dispose()
    //    {
    //        if (disposed)
    //            return;

    //        selectedLocale.Dispose();

    //        disposed = true;
    //    }

    //    public Observable<string> ObserveSelectedLocale()
    //    {
    //        return selectedLocale;
    //    }
    //}
}
