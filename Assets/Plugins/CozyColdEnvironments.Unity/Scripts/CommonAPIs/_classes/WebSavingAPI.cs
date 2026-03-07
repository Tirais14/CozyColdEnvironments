using CCEnvs.Attributes;
using CCEnvs.Unity.Saves;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    //public sealed class WebSavingAPI : ISavingAPI
    //{
    //    private readonly ReactiveProperty<bool> isGameSaving = new();
    //    private readonly ReactiveProperty<bool> isSaveGameLoading = new();

    //    [OnInstallResetable]
    //    public static WebSavingAPI? Instance { get; private set; }

    //    public bool IsGameSaving => isGameSaving.Value;
    //    public bool IsSaveGameLoading => isSaveGameLoading.Value;

    //    public UniTask LoadSaveGameAsync(
    //        string filePath = null!, 
    //        CancellationToken cancellationToken = default
    //        )
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async UniTask SaveGameAsync(
    //        string filePath = null!,
    //        CancellationToken cancellationToken = default
    //        )
    //    {
    //        SavingSystem.Self.Save

    //        isGameSaving.Value = true;
    //    }

    //    private int disposed;
    //    public void Dispose()
    //    {
    //        if (Interlocked.Exchange(ref disposed, 1) != 0)
    //            return;


    //    }

    //    public Observable<bool> ObserveGameSaving()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Observable<bool> ObserveSaveGameLoading()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
