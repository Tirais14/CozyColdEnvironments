#nullable enable
using CCEnvs.Reflection;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Timers
{
    public abstract class TimerMono : CCBehaviour, ITimer
    {
        public const string TIMERS_OBJ_NAME = "___Timers";
        public const string TIMERS_UPDATE_OBJ_NAME = "___Timers_Update";
        public const string TIMERS_FIXED_UPDATE_OBJ_NAME = "___Timers_FixedUpdate";
        public const string TIMERS_LATE_UPDATE_OBJ_NAME = "___Timers_LateUpdate";
        public const string TIMERS_CUSTOM_UPDATE_OBJ_NAME = "___Timers_CustomUpdate";

        protected readonly TimerBase timer = new();

        public TimerOptions Options {
            get => timer.Options;
            set => timer.Options = value;
        }
        public IObservable<TimeSpan> OnTargetReached => timer.OnTargetReached;
        public IObservable<TimeSpan> OnTick => timer.OnTick;

        public bool TargetReached => timer.TargetReached;
        public bool IsEnabled => timer.IsEnabled;
        public bool IsUnscaledInterval { get; set; }
        public TimeSpan Elapsed => timer.Elapsed;
        public TimeSpan? Target {
            get => timer.Target;
            set => timer.Target = value;
        }
        public abstract TimeSpan Interval { get; }

        /// <param name="creationType">Cannot be null if <see cref="UpdateType.Custom"/></param>
        public static ITimer Create(UpdateType updateType, Type? creationType = null)
        {
            var timersGO = GetOrCreateTimersObject();

            if (creationType is not null && creationType.IsNotType<ITimer>())
                throw new ArgumentException(nameof(creationType));

            return updateType switch
            {
                UpdateType.Update => (ITimer)timersGO.Find(TIMERS_UPDATE_OBJ_NAME)!.AddComponent(creationType ?? typeof(TimerUpdate)),
                UpdateType.FixedUpdate => (ITimer)timersGO.Find(TIMERS_FIXED_UPDATE_OBJ_NAME)!.AddComponent(creationType ?? typeof(TimerFixedUpdate)),
                UpdateType.LateUpdate => (ITimer)timersGO.Find(TIMERS_LATE_UPDATE_OBJ_NAME)!.AddComponent(creationType ?? typeof(TimerLateUpdate)),
                UpdateType.Custom => (ITimer)timersGO.Find(TIMERS_CUSTOM_UPDATE_OBJ_NAME)!.AddComponent(creationType ?? throw new ArgumentNullException(nameof(creationType))),
                _ => throw new InvalidOperationException($"{nameof(updateType)} = {updateType}."),
            };
        }
        public static T Create<T>(UpdateType updateType, Type? creationType = null)
            where T : ITimer
        {
            return (T)Create(updateType, creationType);
        }

        private static bool IsValidTimersObject(GameObject go)
        {
            if (go == null)
                return false;

            if (!go.TryFind(TIMERS_UPDATE_OBJ_NAME, out _)
                ||
                !go.TryFind(TIMERS_FIXED_UPDATE_OBJ_NAME, out _)
                ||
                !go.TryFind(TIMERS_LATE_UPDATE_OBJ_NAME, out _)
                ||
                !go.TryFind(TIMERS_CUSTOM_UPDATE_OBJ_NAME, out _))
                return false;

            return true;
        }

        private static GameObject CreateTimerObjects()
        {
            var timersGO = new GameObject(TIMERS_OBJ_NAME);

            var timersUpdateGO = new GameObject(TIMERS_UPDATE_OBJ_NAME);
            var timersFixedUpdateGO = new GameObject(TIMERS_FIXED_UPDATE_OBJ_NAME);
            var timersLateUpdateGO = new GameObject(TIMERS_LATE_UPDATE_OBJ_NAME);
            var timersCustomUpdateGO = new GameObject(TIMERS_CUSTOM_UPDATE_OBJ_NAME);

            timersUpdateGO.transform.parent = timersGO.transform;
            timersFixedUpdateGO.transform.parent = timersGO.transform;
            timersLateUpdateGO.transform.parent = timersGO.transform;
            timersCustomUpdateGO.transform.parent = timersGO.transform;

            DontDestroyOnLoad(timersGO);

            return timersGO;
        }

        private static GameObject GetOrCreateTimersObject()
        {
            var timersGO = GameObject.Find(TIMERS_OBJ_NAME);

            if (!IsValidTimersObject(timersGO))
                timersGO = CreateTimerObjects();

            return timersGO;
        }

        public ITimer StartTimer()
        {
            timer.StartTimer();
            enabled = IsEnabled;

            return this;
        }

        public ITimer StopTimer()
        {
            timer.StopTimer();
            enabled = IsEnabled;

            return this;
        }

        public ITimer ResetTimer()
        {
            return timer.ResetTimer();
        }

        protected void Main() => timer.DoTick();
    }
}
