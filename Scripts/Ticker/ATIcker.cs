using System;
using System.Collections.Generic;
using UTIRLib.Diagnostics;
using UTIRLib.Disposables;
using UTIRLib.Patterns.Commands;

#nullable enable
#pragma warning disable S3881
namespace UTIRLib.Tickables
{
    public abstract class ATIcker<T> 
        :
        MonoX,
        ITickerBase<T>

        where T : ITickableBase
    {
        protected readonly List<T> tickables = new();
        protected readonly Stack<ICommandX> commands = new();

        protected int tickablesCount;

        private void OnDestroy() => ((IDisposable)this).Dispose();

        /// <exception cref="ArgumentNullException"></exception>
        public IDisposable Register(T tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            tickable.OnRegister(this);
            tickables.Add(tickable);
            tickablesCount = tickables.Count;

            return Subscription.Create(this, tickable, (x, y) => x.Unregister(y));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void Unregister(T tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            tickable.OnUnregister(this);
            tickables.Remove(tickable);
            tickablesCount = tickables.Count;
        }

        public void UnregisterAll()
        {
            tickables.Clear();
            tickablesCount = tickables.Count;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void SendCommand(ICommandX command)
        {
            if (command.IsNull())
                throw new ArgumentNullException(nameof(command));

            commands.Push(command);
        }

        void IDisposable.Dispose()
        {
            UnregisterAll();
            GC.SuppressFinalize(this);
        }
    }
}
