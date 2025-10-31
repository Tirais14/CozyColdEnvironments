using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using static CCEnvs.Unity.FindComponentHelper;

#nullable enable
namespace CCEnvs.Unity
{
    public class FindComponentQuery : FindComponentQueryBase<FindComponentQuery>
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<object> Component(Type? type = null)
        {
            return FindComponentRaw(
                Source,
                type: type,
                includeInactive: includeInactive,
                findMode: findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<T> Component<T>()
        {
            return FindComponentRaw<T>(
                Source,
                includeInactive: includeInactive,
                findMode: findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ComponentStrict(Type? type = null)
        {
            return Component(type).IfNone(() => throw new ComponentNotFoundException(type, Source)).Target!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ComponentStricts<T>()
        {
            return Component<T>().IfNone(() => throw new ComponentNotFoundException(typeof(T), Source)).Target!;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> Components(Type? type = null)
        {
            var results = FindComponentsRaw(
                Source,
                type: type,
                includeInactive: includeInactive,
                findMode: findMode);

            Reset();

            return results;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Components<T>()
        {
            var results = FindComponentsRaw<T>(
                Source,
                includeInactive: includeInactive,
                findMode: findMode);

            Reset();

            return results;
        }
    }

    public static class FindComponentQueryExtensions
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FindComponentQuery FindComponent(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return FindComponentQuery.Instance.From(source);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FindComponentQuery FindComponent(this Component source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return FindComponentQuery.Instance.From(source);
        }
    }
}
