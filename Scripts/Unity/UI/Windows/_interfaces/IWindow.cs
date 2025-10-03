using System;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IWindow : IOpenable
    {
        IObservable<IWindow> OnOpen { get; }
        IObservable<IWindow> OnClose { get; }

        bool CanOpen(out string message);
        bool CanOpen();
    }
}
