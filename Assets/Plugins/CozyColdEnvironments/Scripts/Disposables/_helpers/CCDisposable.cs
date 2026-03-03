using System;

#nullable enable
namespace CCEnvs.Disposables
{
    public class CCDisposable
    {
        public static LightDisposable<TState> CreateLight<TState>(
            TState state,
            Action<TState> disposeAction
            )
        {
            return new LightDisposable<TState>(state, disposeAction);
        }

        public static LightDisposable CreateLight(Action disposeAction)
        {
            return new LightDisposable(disposeAction);
        }
    }
}
