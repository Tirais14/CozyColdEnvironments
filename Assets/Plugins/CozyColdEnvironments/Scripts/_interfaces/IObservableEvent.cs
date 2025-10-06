#nullable enable
using System;

namespace CCEnvs
{
    [Obsolete("In developing")]
    public interface IObservableEvent
    {
        public IDisposable AddHandler(Delegate function);

        public void RemoveHandler(Delegate function);
    }
    [Obsolete("In developing")]
    public interface IObservableEvent<in T> : IObservableEvent
        where T : Delegate
    {
        public IDisposable AddHandler(T function);

        public void RemoveHandler(T function);

        IDisposable IObservableEvent.AddHandler(Delegate function)
        {
            if (function is not T typed)
                return CC.Throw.InvalidCast(function.GetType(), typeof(T)).As<IDisposable>();

            return AddHandler(typed);
        }

        void IObservableEvent.RemoveHandler(Delegate function)
        {
            if (function is not T typed)
            {
                CC.Throw.InvalidCast(function.GetType(), typeof(T));
                return;
            }

            RemoveHandler(typed);
        }
    }
}
