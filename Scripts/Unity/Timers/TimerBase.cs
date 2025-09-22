using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Returnables;
using CCEnvs.Rx;
using System;
using System.Diagnostics;

#nullable enable
#pragma warning disable S2328
namespace CCEnvs.Unity.Timers
{
    public sealed class TimerBase : IObserver, ITimer, IEquatable<TimerBase>
    {
        private Stopwatch? intervalWatch;
        private bool isOnTargetReachedInvoked;
        private TimeSpan? interval;

        public IObservable<TimeSpan> OnTargetReached { get; }
        public IObservable<TimeSpan> OnTick { get; }

        public TimerOptions Options { get; set; }
        public TimeSpan Elapsed { get; private set; } = TimeSpan.Zero;
        public TimeSpan? Target { get; set; }
        public TimeSpan Interval => interval ?? intervalWatch!.Elapsed;
        public bool TargetReached => Target is null || Target.Value >= Elapsed;
        public bool IsEnabled { get; private set; }

        public TimerBase()
        {
            OnTick = new Observable<TimeSpan>(Elapsed);
        }

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

        public void DoTick() => this.As<IObserver>().OnNext(default);

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

            CC.Validate.Argument(newInterval, 
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

        void IObserver<Mock>.OnNext(Mock _)
        {
            if (!IsEnabled)
                return;

            Elapsed = Elapsed.Add(Interval);

            ((Observable<TimeSpan>)OnTick).Publish();

            if (!TargetReached && isOnTargetReachedInvoked)
                isOnTargetReachedInvoked = false;

            if (TargetReached
                &&
                !isOnTargetReachedInvoked)
            {
                if (OnTargetReached is not null)
                {
                    OnTargetReached();
                    isOnTargetReachedInvoked = true;
                }

                if (Options.IsFlagSetted(TimerOptions.StopOnTargetReached))
                    StopTimer();

                if (Options.IsFlagSetted(TimerOptions.ResetOnTargetReached))
                    ResetTimer();
            }
        }

        void IObserver<Mock>.OnError(Exception error)
        {
            CCDebug.PrintException(error);
        }

        void IObserver<Mock>.OnCompleted()
        {
        }
    }
}
