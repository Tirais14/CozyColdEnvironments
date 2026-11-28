using System;
using System.Threading;

#nullable enable
namespace CCEnvs.UnityX.Tickables
{
    public struct TickableHandle : IDisposable
    {
        public ITickable? Tickable { get; }

        public IFixedTickable? FixedTickable { get; }

        public ILateTickable? LateTickable { get; }

        internal int TickableIndex { get; }
        internal int FixedTickableIndex { get; }
        internal int LateTickableIndex { get; }

        public ITicker Ticker { get; }

        public bool IsInitialized { get; }

        private int disposed;

        public TickableHandle(
            ITickable? tickable,
            ITicker ticker,
            int index = -1
            )
            : this()
        {
            CC.Guard.IsNotNull(ticker, nameof(ticker));

            this.Tickable = tickable;
            Ticker = ticker;
            TickableIndex = index;

            IsInitialized = true;
        }

        public TickableHandle(
            IFixedTickable? fixedTickable,
            ITicker ticker,
            int index = -1
            )
            : this()
        {
            CC.Guard.IsNotNull(ticker, nameof(ticker));

            this.FixedTickable = fixedTickable;
            Ticker = ticker;
            FixedTickableIndex = index;

            IsInitialized = true;
        }

        public TickableHandle(
            ILateTickable? lateTickable,
            ITicker ticker,
            int index = -1
            )
            : this()
        {
            CC.Guard.IsNotNull(ticker, nameof(ticker));

            this.LateTickable = lateTickable;
            Ticker = ticker;
            LateTickableIndex = index;

            IsInitialized = true;
        }

        public TickableHandle(
            ITickable? tickable,
            IFixedTickable? fixedTickable,
            ILateTickable? lateTickable,
            ITicker ticker,
            int tickableIndex = -1,
            int fixedTickableIndex = -1,
            int lateTickableIndex = -1
            )
            :
            this()
        {
            CC.Guard.IsNotNull(ticker, nameof(ticker));

            this.Tickable = tickable;
            this.TickableIndex = tickableIndex;

            this.FixedTickable = fixedTickable;
            this.FixedTickableIndex = fixedTickableIndex;

            this.LateTickable = lateTickable;
            this.LateTickableIndex = lateTickableIndex;

            Ticker = ticker;

            IsInitialized = true;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0
                || !IsInitialized
                )
                return;

            Ticker.Unregister(this);
        }
    }
}
