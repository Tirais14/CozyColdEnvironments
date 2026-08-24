using R3;
using System;
using System.Threading;

#nullable enable
namespace CCEnvs.Proeprties
{
    public class Toggle : IToggle, IDisposable
    {
        private readonly ReactiveProperty<bool> state = new();

        public bool State {
            get => state.Value;
            set => state.Value = value;
        }

        public Toggle() { }

        public Toggle(bool initialState)
        {
            state.Value = initialState;
        }

        ~Toggle() => Dispose();

        public static implicit operator bool(Toggle instance)
        {
            return instance.State;
        }

        public bool Trigger()
        {
            State = !State;
            return State;
        }

        public Observable<bool> ObserveState() => state;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        private int disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (disposing)
                state.Dispose();
        }

        public override string ToString() => State.ToString();
    }
}
