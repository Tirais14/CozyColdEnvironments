using System;
using System.Collections;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.Utils
{
    [DefaultExecutionOrder(-7777)]
    public sealed class CoroutineTask : MonoXStatic<CoroutineTask>
    {
        public enum CallType
        {
            None,
            ForSeconds,
            OnFrameEnd,
            OnPredicate,
        }

        private static readonly string TaskStartedMessage = $"{typeof(CoroutineTask).Name.InsertWhitespacesByCase()} {{0}} started.";
        private static readonly string TaskEndedMessage = $"{typeof(CoroutineTask).Name.InsertWhitespacesByCase()} {{0}} ended.";

#nullable disable
        public static void Run(IEnumerator routine)
        {
            instance.StartCoroutine(routine);
        }

        /// <param name="callArgs">
        /// <br/><see cref="CallType.ForSeconds"/>: 0 (time) = <see cref="Single"/>, 1 (realtime) = <see cref="Boolean"/>
        /// <br/><see cref="CallType.OnPredicate"/>: <see cref="Func{Boolean}"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Run(CallType callType, Action action, params object[] callArgs)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            instance.RunInternal(callType, action, callArgs);
        }

        /// <param name="callArgs">
        /// <br/><see cref="CallType.ForSeconds"/>: 0 (time) = <see cref="Single"/>, 1 (realtime) = <see cref="Boolean"/>
        /// <br/><see cref="CallType.OnPredicate"/>: <see cref="Func{Boolean}"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Run<T>(CallType callType, Action<T> action, T arg, params object[] callArgs)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            instance.RunInternal(callType, action, callArgs, arg);
        }

        /// <param name="callArgs">
        /// <br/><see cref="CallType.ForSeconds"/>: 0 (time) = <see cref="Single"/>, 1 (realtime) = <see cref="Boolean"/>
        /// <br/><see cref="CallType.OnPredicate"/>: <see cref="Func{Boolean}"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Run<T0, T1>(CallType callType,
                                       Action<T0, T1> action,
                                       T0 arg0,
                                       T1 arg1,
                                       params object[] callArgs)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            instance.RunInternal(callType, action, callArgs, arg0, arg1);
        }

        /// <param name="callArgs">
        /// <br/><see cref="CallType.ForSeconds"/>: 0 (time) = <see cref="Single"/>, 1 (realtime) = <see cref="Boolean"/>
        /// <br/><see cref="CallType.OnPredicate"/>: <see cref="Func{Boolean}"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Run<T0, T1, T2>(CallType callType,
                                           Action<T0, T1, T2> action,
                                           T0 arg0,
                                           T1 arg1,
                                           T2 arg2,
                                           params object[] callArgs)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            instance.RunInternal(callType, action, callArgs, arg0, arg1, arg2);
        }

        /// <param name="callArgs">
        /// <br/><see cref="CallType.ForSeconds"/>: 0 (time) = <see cref="Single"/>, 1 (realtime) = <see cref="Boolean"/>
        /// <br/><see cref="CallType.OnPredicate"/>: <see cref="Func{Boolean}"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Run<T0, T1, T2, T3>(CallType callType,
                                               Action<T0, T1, T2, T3> action,
                                               T0 arg0,
                                               T1 arg1,
                                               T2 arg2,
                                               T3 arg3,
                                               params object[] callArgs)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            instance.RunInternal(callType, action, callArgs, arg0, arg1, arg2, arg3);
        }

        /// <param name="callArgs">
        /// <br/><see cref="CallType.ForSeconds"/>: 0 (time) = <see cref="Single"/>, 1 (realtime) = <see cref="Boolean"/>
        /// <br/><see cref="CallType.OnPredicate"/>: <see cref="Func{Boolean}"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Run<T0, T1, T2, T3, T4>(CallType callType,
                                                   Action<T0, T1, T2, T3, T4> action,
                                                   T0 arg0,
                                                   T1 arg1,
                                                   T2 arg2,
                                                   T3 arg3,
                                                   T4 arg4,
                                                   params object[] callArgs)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            instance.RunInternal(callType, action, callArgs, arg0, arg1, arg2, arg3, arg4);
        }

        /// <param name="callArgs">
        /// <br/><see cref="CallType.ForSeconds"/>: 0 (time) = <see cref="Single"/>, 1 (realtime) = <see cref="Boolean"/>
        /// <br/><see cref="CallType.OnPredicate"/>: <see cref="Func{Boolean}"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Run<T0, T1, T2, T3, T4, T5>(CallType callType,
                                                       Action<T0, T1, T2, T3, T4, T5> action,
                                                       T0 arg0,
                                                       T1 arg1,
                                                       T2 arg2,
                                                       T3 arg3,
                                                       T4 arg4,
                                                       T5 arg5,
                                                       params object[] callArgs)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            instance.RunInternal(callType, action, callArgs, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        /// <param name="callArgs">
        /// <br/><see cref="CallType.ForSeconds"/>: 0 (time) = <see cref="Single"/>, 1 (realtime) = <see cref="Boolean"/>
        /// <br/><see cref="CallType.OnPredicate"/>: <see cref="Func{Boolean}"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Run<T0, T1, T2, T3, T4, T5, T6>(
            CallType callType,
            Action<T0, T1, T2, T3, T4, T5, T6> action,
            T0 arg0,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            params object[] callArgs)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            instance.RunInternal(callType, action, callArgs, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <param name="callArgs">
        /// <br/><see cref="CallType.ForSeconds"/>: 0 (time) = <see cref="Single"/>, 1 (realtime) = <see cref="Boolean"/>
        /// <br/><see cref="CallType.OnPredicate"/>: <see cref="Func{Boolean}"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Run<T0, T1, T2, T3, T4, T5, T6, T7>(
            CallType callType,
            Action<T0, T1, T2, T3, T4, T5, T6, T7> action,
            T0 arg0,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            params object[] callArgs)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            instance.RunInternal(callType,
                                 action,
                                 callArgs,
                                 arg0,
                                 arg1,
                                 arg2,
                                 arg3,
                                 arg4,
                                 arg5,
                                 arg6,
                                 arg7);
        }

        private void RunInternal(CallType callType,
                                 Delegate action,
                                 object[] callArgs,
                                 params object[] args)
        {
            switch (callType)
            {
                case CallType.ForSeconds:
                    if (callArgs.IsEmpty()) throw new WrongCollectionException(callArgs);

                    StartCoroutine(TimedRoutine(action, (float)callArgs[0], (bool)callArgs[1], args));
                    break;

                case CallType.OnFrameEnd:
                    StartCoroutine(EndFrameRoutine(action, args));
                    break;

                case CallType.OnPredicate:
                    if (callArgs.IsEmpty()) throw new WrongCollectionException(callArgs);

                    StartCoroutine(PredicateRoutine(action, (Func<bool>)callArgs[0], args));
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        private static IEnumerator TimedRoutine(Delegate action,
                                                float seconds,
                                                bool realtime,
                                                object[] args)
        {
            TirLibDebug.LogFormat(TaskStartedMessage, instance, nameof(TimedRoutine));

            if (realtime)
            {
                yield return new WaitForSecondsRealtime(seconds);
            }
            else yield return new WaitForSeconds(seconds);

            action.DynamicInvoke(args);

            TirLibDebug.LogFormat(TaskEndedMessage, instance, nameof(TimedRoutine));
        }

        private static IEnumerator EndFrameRoutine(Delegate action, object[] args)
        {
            TirLibDebug.LogFormat(TaskStartedMessage, instance, nameof(EndFrameRoutine));

            yield return new WaitForEndOfFrame();

            action.DynamicInvoke(args);

            TirLibDebug.LogFormat(TaskEndedMessage, instance, nameof(EndFrameRoutine));
        }

        private static IEnumerator PredicateRoutine(Delegate action, Func<bool> predicate, object[] args)
        {
            TirLibDebug.LogFormat(TaskStartedMessage, instance, nameof(PredicateRoutine));

            yield return new WaitUntil(predicate);

            action.DynamicInvoke(args);

            TirLibDebug.LogFormat(TaskEndedMessage, instance, nameof(PredicateRoutine));
        }

#nullable enable

        private void OnDisable() => enabled = true;
    }
}