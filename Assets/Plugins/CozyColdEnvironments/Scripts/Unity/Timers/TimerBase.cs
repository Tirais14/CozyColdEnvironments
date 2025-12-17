using CCEnvs.Diagnostics;
using CCEnvs.Returnables;
using System;
using System.Diagnostics;
using R3;

#nullable enable
#pragma warning disable S2328
namespace CCEnvs.Unity.Timers
{
    public sealed class TimerBase : IObserver<Returnables.Unit>, ITimer, IEquatable<TimerBase>
    {
        private readonly Subject<TimeSpan> onTargetReached = new();
        private readonly Subject<TimeSpan> onTick = new();
        private Stopwatch? intervalWatch;
        private bool isOnTargetReachedInvoked;
        private TimeSpan? interval;

        public Observable<TimeSpan> OnTargetReached => onTargetReached.AsObservable();
        public Observable<TimeSpan> OnTick => onTick.AsObservable();

        public TimerOptions Options { get; set; }
        public TimeSpan Elapsed { get; private set; } = TimeSpan.Zero;
        public TimeSpan? Target { get; set; }
        public TimeSpan Interval => interval ?? intervalWatch!.Elapsed;
        public bool TargetReached => Target is null || Target.Value >= Elapsed;
        public bool IsEnabled { get; private set; }


        public static implicit operator bool(TimerBase timer)
        {
            return timer is not null && timer.TargetReached;
        }

        public static bool operator ==(TimerBase? left, TimerBase? right)
        {
            if (left is null && right is null)
                return true;
            if (left is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(TimerBase? left, TimerBase? right)
        {
            if (left is null && right is null)
                return false;
            if (left is null)
                return true;

            return !left.Equals(right);
        }

        public void DoTick() => this.To<IObserver<Returnables.Unit>>().OnNext(default);

        public ITimer StartTimer()
        {
            IsEnabled = true;

            return this;
        }

        public ITimer StopTimer()
        {
            IsEnabled = false;

            return this;
        }

        public ITimer ResetTimer()
        {
            Elapsed = TimeSpan.Zero;
            isOnTargetReachedInvoked = false;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newInterval">If null, uses stopwatch to calculate</param>
        public void SetInterval(TimeSpan? newInterval)
        {
            if (newInterval is null)
            {
                interval = null;
                intervalWatch = new Stopwatch();
                return;
            }

            CC.Guard.ArgumentObsolete(newInterval,
                                 nameof(newInterval),
                                 newInterval > TimeSpan.Zero);

            interval = newInterval;
        }

        public bool Equals(TimerBase? other)
        {
            if (other is null)
                return false;

            return Elapsed == other.Elapsed && Target == other.Target;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as TimerBase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Elapsed, Target);
        }

        void IObserver<Returnables.Unit>.OnNext(Returnables.Unit _)
        {
            if (!IsEnabled)
                return;

            Elapsed = Elapsed.Add(Interval);

            onTick.OnNext(Interval);

            if (!TargetReached && isOnTargetReachedInvoked)
                isOnTargetReachedInvoked = false;

            if (TargetReached
                &&
                !isOnTargetReachedInvoked)
            {
                if (OnTargetReached is not null)
                {
                    onTargetReached.OnNext(Elapsed);
                    isOnTargetReachedInvoked = true;
                }

                if (Options.IsFlagSetted(TimerOptions.StopOnTargetReached))
                    StopTimer();

                if (Options.IsFlagSetted(TimerOptions.ResetOnTargetReached))
                    ResetTimer();
            }
        }

        void IObserver<Returnables.Unit>.OnError(Exception error)
        {
            CCDebug.Instance.PrintException(error);
        }

        void IObserver<Returnables.Unit>.OnCompleted()
        {
        }
    }
}
