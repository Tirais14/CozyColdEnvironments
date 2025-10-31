using CCEnvs.Diagnostics;
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
    public class FindComponentQuery
    {
        public readonly static FindComponentQuery Instance = new();

        public static FindComponentQuery Empty => new();

        public GameObject Source { get; protected set; } = null!;
        public bool includeInactive { get; protected set; }
        public FindMode findMode { get; protected set; } = FindMode.Self;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FindComponentQuery From(GameObject gameObject)
        {
            if (gameObject == null)
            {
                this.PrintError($"{nameof(gameObject)} is null.");
                return this;
            }

            Source = gameObject;
            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FindComponentQuery From(Component component)
        {
            if (component == null)
            {
                this.PrintError($"{nameof(component)} is null.");
                return this;
            }

            Source = component.gameObject;
            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FindComponentQuery IncludeInactive()
        {
            includeInactive = true;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FindComponentQuery InSelf()
        {
            findMode = FindMode.Self;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FindComponentQuery InChildren()
        {
            findMode = FindMode.InChilds;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FindComponentQuery InParent()
        {
            findMode = FindMode.InParents;

            return this;
        }

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
            return FindComponentsRaw(
                Source,
                type: type,
                includeInactive: includeInactive,
                findMode: findMode);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Components<T>()
        {
            return FindComponentsRaw<T>(
                Source,
                includeInactive: includeInactive,
                findMode: findMode);
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
