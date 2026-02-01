using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public interface ISavingAPI : IDisposable
    {
        bool IsGameSaving { get; }
        bool IsSaveGameLoading { get; }

        UniTask SaveGameAsync(string? filePath = null, CancellationToken cancellationToken = default);

        UniTask LoadSaveGameAsync(string? filePath = null, CancellationToken cancellationToken = default);

        Observable<bool> ObserveGameSaving();

        Observable<bool> ObserveGameSaved();

        Observable<bool> ObserveSaveGameLoading();

        Observable<bool> ObserveSaveGameLoaded();
    }
}
