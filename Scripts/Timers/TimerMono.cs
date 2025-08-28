#nullable enable
using System;
using UnityEngine;
using UTIRLib.Unity;

namespace UTIRLib.Timers
{
    [DisallowMultipleComponent]
    public class TimerMono : MonoX, ITimer
    {
        public const string TIMERS_OBJ_NAME = "___Timers";
        public const string TIMERS_UPDATE_OBJ_NAME = "___Timers_Update";
        public const string TIMERS_FIXED_UPDATE_OBJ_NAME = "___Timers_FixedUpdate";
        public const string TIMERS_LATE_UPDATE_OBJ_NAME = "___Timers_LateUpdate";

        protected readonly TimerManual timer = new();

        public event Action OnTargetReached {
            add => timer.OnTargetReached += value;
            remove => timer.OnTargetReached -= value;
        }

        public ITimer Timer => timer;
        public float Seconds => timer.Seconds;
        public float TargetValue {
            get => timer.TargetValue;
            set => timer.TargetValue = value;
        }
        public bool TargetValueReached => timer.TargetValueReached;
        public bool IsActive => timer.IsActive;
        public bool IsOnTargetReachedInvoked => timer.IsOnTargetReachedInvoked;
        public TimerOptions Options {
            get => timer.Options;
            set => timer.Options = value;
        }

        public TimeSpan GetTimeSpan() => timer.GetTimeSpan();

        public ITimer StartTimer()
        {
            timer.StartTimer();
            enabled = IsActive;

            return this;
        }

        public ITimer StopTimer()
        {
            timer.StopTimer();
            enabled = IsActive;

            return this;
        }

        public ITimer ResetTimer()
        {
            return timer.ResetTimer();
        }

        public static ITimer Create(UpdateType updateType)
        {
            var timersGO = GetOrCreateTimersObject();

            return updateType switch
            {
                UpdateType.Update => timersGO.Find(TIMERS_UPDATE_OBJ_NAME)!.AddComponent<TimerUpdate>(),
                UpdateType.FixedUpdate => timersGO.Find(TIMERS_FIXED_UPDATE_OBJ_NAME)!.AddComponent<TimerFixedUpdate>(),
                UpdateType.LateUpdate => timersGO.Find(TIMERS_LATE_UPDATE_OBJ_NAME)!.AddComponent<TimerLateUpdate>(),
                _ => throw new InvalidOperationException($"{nameof(updateType)} = {updateType}."),
            };
        }

        private static bool IsValidTimersObject(GameObject go)
        {
            if (go == null)
                return false;

            if (!go.TryFind(TIMERS_UPDATE_OBJ_NAME, out _)
                ||
                !go.TryFind(TIMERS_FIXED_UPDATE_OBJ_NAME, out _)
                ||
                !go.TryFind(TIMERS_LATE_UPDATE_OBJ_NAME, out _))
                return false;

            return true;
        }

        private static GameObject CreateTimerObject()
        {
            var timersGO = new GameObject(TIMERS_OBJ_NAME);

            var timersUpdateGO = new GameObject(TIMERS_UPDATE_OBJ_NAME);
            var timersFixedUpdateGO = new GameObject(TIMERS_FIXED_UPDATE_OBJ_NAME);
            var timersLateUpdateGO = new GameObject(TIMERS_LATE_UPDATE_OBJ_NAME);

            timersUpdateGO.transform.parent = timersGO.transform;
            timersFixedUpdateGO.transform.parent = timersGO.transform;
            timersLateUpdateGO.transform.parent = timersGO.transform;

            DontDestroyOnLoad(timersGO);

            return timersGO;
        }

        private static GameObject GetOrCreateTimersObject()
        {
            var timersGO = GameObject.Find(TIMERS_OBJ_NAME);

            if (!IsValidTimersObject(timersGO))
                timersGO = CreateTimerObject();

            return timersGO;
        }
    }
}
