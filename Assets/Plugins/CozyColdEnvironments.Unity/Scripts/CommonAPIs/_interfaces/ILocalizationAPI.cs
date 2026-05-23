using R3;
using System;

#nullable enable
namespace CCEnvs.UnityX.CommonAPIs
{
    public interface ILocalizationAPI : IDisposable
    {
        string SelectedLocale { get; }

        void SetLocale(string code);

        Observable<string> ObserveSelectedLocale();
    }
}
