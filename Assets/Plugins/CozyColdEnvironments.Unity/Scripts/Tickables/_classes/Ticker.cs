using CCEnvs.Disposables;
using CCEnvs.UnityX.Components;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Tickables
{
    public class Ticker : CCBehaviour
    {
        private readonly List<ITickable> tickables = new();

        private readonly List<IFixedTickable> fixedTickables = new();

        private readonly List<ILateTickable> lateTickables = new();

        private void Update()
        {
            for (int i = 0; i < tickables.Count; i++)
            {
                try
                {
                    tickables[i].Tick();
                }
                catch (System.Exception ex)
                {
                    this.PrintException(ex);
                }
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < fixedTickables.Count; i++)
            {
                try
                {
                    fixedTickables[i].FixedTick();
                }
                catch (System.Exception ex)
                {
                    this.PrintException(ex);
                } 
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < lateTickables.Count; i++)
            {
                try
                {
                    lateTickables[i].LateTick();
                }
                catch (System.Exception ex)
                {
                    this.PrintException(ex);
                }
            }
        }

        public static Ticker Create(string? name)
        {
            return new GameObject(name ?? $"___Ticker{Random.Range(0, int.MaxValue)}", typeof(Ticker))
                .Q()
                .Component<Ticker>()
                .Strict();
        }

        public bool Unregister(ITickable? tickable)
        {
            if (tickable is null)
                return false;

            return tickables.Remove(tickable);
        }
        public bool Unregister(IFixedTickable? fixedTickable)
        {
            if (fixedTickable is null)
                return false;

            return fixedTickables.Remove(fixedTickable);
        }
        public bool Unregister(ILateTickable? lateTickable)
        {
            if (lateTickable is null)
                return false;

            return lateTickables.Remove(lateTickable);
        }

        public DisposableLight<(Ticker Ticker, ITickable Tickable)> Register(ITickable tickable)
        {
            CC.Guard.IsNotNull(tickable, nameof(tickable));
            tickables.Add(tickable);
            return CCDisposable.Light((ticker: this, tickable), static args => args.ticker.tickables.Remove(args.tickable));
        }
        public DisposableLight<(Ticker Ticker, IFixedTickable FixedTickable)> Register(IFixedTickable fixedTickable)
        {
            CC.Guard.IsNotNull(fixedTickable, nameof(fixedTickable));
            fixedTickables.Add(fixedTickable);
            return CCDisposable.Light((ticker: this, fixedTickable), static args => args.ticker.fixedTickables.Remove(args.fixedTickable));
        }
        public DisposableLight<(Ticker Ticker, ILateTickable LateTickable)> Register(ILateTickable lateTickable)
        {
            CC.Guard.IsNotNull(lateTickable, nameof(lateTickable));
            lateTickables.Add(lateTickable);
            return CCDisposable.Light((ticker: this, lateTickable), static args => args.ticker.lateTickables.Remove(args.lateTickable));
        }
    }
}
