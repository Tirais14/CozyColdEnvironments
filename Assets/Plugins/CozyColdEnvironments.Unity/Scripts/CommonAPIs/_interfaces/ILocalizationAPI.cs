using System;
using R3;

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
