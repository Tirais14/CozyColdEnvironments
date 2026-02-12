using CCEnvs.Caching;
using CCEnvs.Collections;
using CCEnvs.Reflection.Caching;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CCEnvs.Linq;

#nullable enable
namespace CCEnvs.Reflection
{
    public struct ReflectionMethodHandle : IEquatable<ReflectionMethodHandle>
    {
        private readonly static Cache<ReflectionMethodHandle, MethodKey> methodKeys = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private readonly static Cache<ReflectionMethodHandle, MethodKey> ctorKeys = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private int? hashCode;

        public ReflectionHandle Core { readonly get; init; }

        public StructuralArray<ParameterKey> ParameterKeys { readonly get; init; }

        public Type? ReturnType { readonly get; init; }

        public ReflectionMethodHandle(ReflectionHandle core)
            :
            this()
        {
            Core = core;
        }

        public static ReflectionMethodHandle Create()
        {
            return new ReflectionMethodHandle
            {
                ParameterKeys = StructuralArray<ParameterKey>.Empty
            };
        }

        public static bool operator ==(ReflectionMethodHandle left, ReflectionMethodHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReflectionMethodHandle left, ReflectionMethodHandle right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle WithCore(ReflectionHandle core)
        {
            return new ReflectionMethodHandle
            {
                Core = core,
                ParameterKeys = ParameterKeys,
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle WithParameterKeys(params ParameterKey[] paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectionMethodHandle
            {
                Core = Core,
                ParameterKeys = paramKeys.ToStructuralArray(),
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle WithParameterKeys(IEnumerable<ParameterKey> paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectionMethodHandle
            {
                Core = Core,
                ParameterKeys = paramKeys.ToStructuralArray(),
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle WithParameterKeys(StructuralArray<ParameterKey> paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectionMethodHandle
            {
                Core = Core,
                ParameterKeys = paramKeys,
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle WithReturnType(Type? returnType)
        {
            return new ReflectionMethodHandle
            {
                Core = Core,
                ParameterKeys = ParameterKeys,
                ReturnType = returnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsReturnTypeMatch(Type? returnType)
        {
            if (ReturnType is null)
                return true;

            return ReturnType.IsType(returnType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerable<MethodInfo> GetMethods()
        {
            return new MethodsEnumerator(
                this,
                Core.GetMembers(MemberTypes.Method).Cast<MethodBase>()
                )
                .Cast<MethodInfo>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodInfo? GetMethod(bool throwIfNotFound)
        {
            if (methodKeys.TryGet(this, out var methodKey)
                &&
                CachedMembers.TryGetMethod(methodKey, out var method))
            {
                return method;
            }

            if (!GetMethods().SingleOrDefault().Let(out method))
            {
                if (throwIfNotFound)
                    throw GetCannotFindMethodException();

                return method;
            }

            if (Core.CacheResults)
                CacheMethod(method);

            return method;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerable<ConstructorInfo> GetConstructors()
        {
            return new MethodsEnumerator(
                this,
                Core.GetMembers(MemberTypes.Constructor).Cast<MethodBase>()
                )
                .Cast<ConstructorInfo>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ConstructorInfo? GetConstructor(bool throwIfNotFound)
        {
            if (ctorKeys.TryGet(this, out var ctorKey)
                &&
                CachedMembers.TryGetConstructor(ctorKey, out var ctor))
            {
                return ctor;
            }

            if (!GetConstructors().SingleOrDefault().Let(out ctor))
            {
                if (throwIfNotFound)
                    throw GetCannotFindMethodException();

                return ctor;
            }

            if (Core.CacheResults)
                CacheConstructor(ctor);

            return ctor;
        }

        public readonly bool Equals(ReflectionMethodHandle other)
        {
            return Core == other.Core
                   &&
                   ParameterKeys == other.ParameterKeys
                   &&
                   ReturnType == other.ReturnType;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReflectionMethodHandle typed && Equals(typed);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Core)}: {Core}; {nameof(ParameterKeys)}: {ParameterKeys}; {nameof(ReturnType)}: {ReturnType})";
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Core, ParameterKeys, ReturnType);

            return hashCode.Value;
        }

        private readonly InvalidOperationException GetCannotFindMethodException()
        {
            return new InvalidOperationException($"Cannot find any method by {this}");
        }

        private readonly void CacheMethod(MethodInfo method)
        {
            if (methodKeys.TryAdd(this, new MethodKey(method), out var entry))
                entry.ExpirationTimeRelativeToNow = 20.Minutes();

            CachedMembers.TryAddMethod(method);
        }

        private readonly void CacheConstructor(ConstructorInfo ctor)
        {
            if (ctorKeys.TryAdd(this, new MethodKey(ctor), out var entry))
                entry.ExpirationTimeRelativeToNow = 20.Minutes();

            CachedMembers.TryAddConstructor(ctor);
        }

        public struct MethodsEnumerator : IEnumerator<MethodBase?>, IEnumerable<MethodBase>
        {
            private readonly ReflectionMethodHandle reflectedHandle;

            private readonly IEnumerator<MethodBase> sourceEnumerator;

            public MethodBase? Current { get; private set; }

            readonly object? IEnumerator.Current => Current;

            public MethodsEnumerator(
                ReflectionMethodHandle handle,
                IEnumerable<MethodBase> source
                )
                :
                this()
            {
                CC.Guard.IsNotNull(source, nameof(source));

                this.reflectedHandle = handle;
                sourceEnumerator = source.GetEnumerator();
            }

            public bool MoveNext()
            {
                while (sourceEnumerator.TryMoveNext(out var tCurrent))
                {
                    if (!reflectedHandle.Core.IsNameMatch(tCurrent.Name))
                        continue;

                    if (!tCurrent.IsConstructor)
                    {
                        if (!reflectedHandle.IsReturnTypeMatch(((MethodInfo)tCurrent).ReturnType))
                            continue;
                    }

                    Current = tCurrent;
                    return true;
                }

                return false;
            }

            public readonly IEnumerator<MethodBase> GetEnumerator() => this;

            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            readonly void IEnumerator.Reset()
            {
                throw new NotSupportedException("Not resetable");
            }

            readonly void IDisposable.Dispose()
            {
            }
        }
    }
}
