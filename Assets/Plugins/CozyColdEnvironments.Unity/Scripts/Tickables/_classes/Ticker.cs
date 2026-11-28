using CCEnvs.Disposables;
using CCEnvs.UnityX.Components;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Tickables
{
    public class Ticker : CCBehaviour, ITicker
    {
        private readonly List<ITickable> tickables = new();

        private readonly List<IFixedTickable> fixedTickables = new();

        private readonly List<ILateTickable> lateTickables = new();

        private void Update()
        {
            for (int i = 0; i < tickables.Count; i++)
            {
                var tickable = tickables[i];

                if (tickable is ITickableToggleable toggleable
                    && !toggleable.CanTick
                    )
                    continue;

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
                var tickable = fixedTickables[i];

                if (tickable is ITickableToggleable toggleable
                    && !toggleable.CanTick
                    )
                    continue;

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
                var tickable = lateTickables[i];

                if (tickable is ITickableToggleable toggleable
                    && !toggleable.CanTick
                    )
                    continue;

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
        public bool Unregister(object? obj)
        {
            if (obj is null)
                return false;

            bool isUnregistered = false;

            if (obj is ITickable tickable && Unregister(tickable))
                isUnregistered = true;

            if (obj is IFixedTickable fixedTickable && Unregister(fixedTickable))
                isUnregistered = true;

            if (obj is ILateTickable lateTickable && Unregister(lateTickable))
                isUnregistered = true;

            return isUnregistered;
        }
        public bool Unregister(TickableHandle handle)
        {
            if (!handle.IsInitialized)
                return false;

            bool isUnregistered = false;

            if (handle.Tickable is not null)
            {
                if (handle.TickableIndex >= 0
                    && handle.TickableIndex < tickables.Count
                    && handle.Tickable.Equals(tickables[handle.TickableIndex])
                    )
                    tickables.RemoveAt(handle.TickableIndex);
                else if (Unregister(handle.Tickable))
                    isUnregistered = true;
            }

            if (handle.FixedTickable is not null)
            {
                if (handle.FixedTickableIndex >= 0
                    && handle.FixedTickableIndex < fixedTickables.Count
                    && handle.FixedTickable.Equals(fixedTickables[handle.FixedTickableIndex])
                    )
                    fixedTickables.RemoveAt(handle.FixedTickableIndex);
                else if (Unregister(handle.FixedTickable))
                    isUnregistered = true;
            }

            if (handle.LateTickable is not null)
            {
                if (handle.LateTickableIndex >= 0
                    && handle.LateTickableIndex < lateTickables.Count
                    && handle.LateTickable.Equals(lateTickables[handle.LateTickableIndex])
                    )
                    lateTickables.RemoveAt(handle.LateTickableIndex);
                else if (Unregister(handle.LateTickable))
                    isUnregistered = true;
            }

            return isUnregistered;
        }

        public TickableHandle Register(ITickable tickable)
        {
            CC.Guard.IsNotNull(tickable, nameof(tickable));
            tickables.Add(tickable);
            return new TickableHandle(tickable, this, tickables.Count - 1);
        }
        public TickableHandle Register(IFixedTickable fixedTickable)
        {
            CC.Guard.IsNotNull(fixedTickable, nameof(fixedTickable));
            fixedTickables.Add(fixedTickable);
            return new TickableHandle(fixedTickable, this, fixedTickables.Count - 1);
        }
        public TickableHandle Register(ILateTickable lateTickable)
        {
            CC.Guard.IsNotNull(lateTickable, nameof(lateTickable));
            lateTickables.Add(lateTickable);
            return new TickableHandle(lateTickable, this, lateTickables.Count - 1);
        }

        public TickableHandle Register(object obj)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            var tickable = obj as ITickable;
            var fixedTickable = obj as IFixedTickable;
            var lateTickable = obj as ILateTickable;

            if (tickable is not null)
                Register(tickable);

            if (fixedTickable is not null)
                Register(fixedTickable);

            if (lateTickable is not null)
                Register(lateTickable);

            return new TickableHandle(
                tickable,
                fixedTickable,
                lateTickable,
                this,
                tickables.Count - 1,
                fixedTickables.Count - 1,
                lateTickables.Count - 1
                );
        }
    }
}
