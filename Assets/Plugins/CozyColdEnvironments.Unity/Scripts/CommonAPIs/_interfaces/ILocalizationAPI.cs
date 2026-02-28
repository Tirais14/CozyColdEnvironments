using R3;
using System;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    public interface ILocalizationAPI : IDisposable
    {
        string SelectedLocale { get; }

        void SetLocale(string code);

        Observable<string> ObserveSelectedLocale();
    }
}
