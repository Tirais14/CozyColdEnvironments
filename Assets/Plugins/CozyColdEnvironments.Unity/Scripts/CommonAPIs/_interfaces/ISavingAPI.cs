using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    public interface ISavingAPI : IDisposable
    {
        bool IsGameSaving { get; }
        bool IsSaveGameLoading { get; }

        UniTask SaveGameAsync(string? filePath = null, CancellationToken cancellationToken = default);

        UniTask LoadSaveGameAsync(string? filePath = null, CancellationToken cancellationToken = default);

        Observable<bool> ObserveGameSaving();

        Observable<bool> ObserveSaveGameLoading();
    }
}
