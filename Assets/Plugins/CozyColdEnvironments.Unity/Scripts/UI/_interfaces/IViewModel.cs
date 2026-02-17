#nullable enable
#pragma warning disable IDE1006
using System;
using System.Threading;

namespace CCEnvs.Unity.UI
{
    public interface IViewModel : IDisposable
    {
        object model { get; }

        CancellationToken DisposeCancellationToken { get; }
    }
    public interface IViewModel<TModel> : IViewModel
    {
        new TModel model { get; }

        object IViewModel.model => model!;
    }
}
