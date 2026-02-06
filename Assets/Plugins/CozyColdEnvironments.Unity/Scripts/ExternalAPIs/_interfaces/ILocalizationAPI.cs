using R3;
using System;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public interface ILocalizationAPI : IDisposable
    {
        string SelectedLocale { get; }

        void SetLocale(string code);

        Observable<string> ObserveSelectedLocale();
    }
}
