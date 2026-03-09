using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CCEnvs.Caching;
using CCEnvs.Collections;
using CCEnvs.Linq;
using CCEnvs.Reflection.Caching;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using Humanizer;

#nullable enable
namespace CCEnvs.Reflection
{
    public struct ReflectionMethodHandle : IEquatable<ReflectionMethodHandle>
    {
        private readonly static Cache<ReflectionMethodHandle, MethodKey> cachedMethodKeys = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private readonly static Cache<ReflectionMethodHandle, MethodKey> cachedCtorKeys = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private int? hashCode;

        public ReflectionHandle BaseReflectionHandle { readonly get; init; }

        public StructuralArray<ParameterKey> ParameterKeys { readonly get; init; }

        public Type? ReturnTypeFilter { readonly get; init; }

        public ReflectionMethodHandle(ReflectionHandle baseReflectionHandle)
            :
            this()
        {
            BaseReflectionHandle = baseReflectionHandle;
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
        public readonly ReflectionMethodHandle WithBaseReflectionHandle(ReflectionHandle baseReflectionHandle)
        {
            return new ReflectionMethodHandle
            {
                BaseReflectionHandle = baseReflectionHandle,
                ParameterKeys = ParameterKeys,
                ReturnTypeFilter = ReturnTypeFilter
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle WithParameterKeys(params ParameterKey[] paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectionMethodHandle
            {
                BaseReflectionHandle = BaseReflectionHandle,
                ParameterKeys = paramKeys.ToStructuralArray(),
                ReturnTypeFilter = ReturnTypeFilter
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle WithParameterKeys(IEnumerable<ParameterKey> paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectionMethodHandle
            {
                BaseReflectionHandle = BaseReflectionHandle,
                ParameterKeys = paramKeys.ToStructuralArray(),
                ReturnTypeFilter = ReturnTypeFilter
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle WithParameterKeys(StructuralArray<ParameterKey> paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectionMethodHandle
            {
                BaseReflectionHandle = BaseReflectionHandle,
                ParameterKeys = paramKeys,
                ReturnTypeFilter = ReturnTypeFilter
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle WithReturnTypeFilter(Type? returnTypeFilter)
        {
            return new ReflectionMethodHandle
            {
                BaseReflectionHandle = BaseReflectionHandle,
                ParameterKeys = ParameterKeys,
                ReturnTypeFilter = returnTypeFilter
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle WithReturnTypeFilter<T>()
        {
            return WithReturnTypeFilter(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsReturnTypeMatch(Type? returnType)
        {
            if (ReturnTypeFilter is null)
                return true;

            return returnType.IsType(ReturnTypeFilter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerable<MethodInfo> GetMethods()
        {
            return new MethodsEnumerator(
                this,
                BaseReflectionHandle.GetMembers(MemberTypes.Method)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodInfo? GetMethod(bool throwIfNotFound)
        {
            if (cachedMethodKeys.TryGetValue(this, out var methodKey)
                &&
                CachedMembers.TryGetMethod(methodKey, out var method))
            {
                return method;
            }

            if (GetMethods().SingleOrDefault().IsNot(out method))
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException($"Cannot find any method by {nameof(ReflectionMethodHandle)}: {this}");

                return method;
            }

            if (BaseReflectionHandle.CacheResults)
                CacheMethod(method);

            return method;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly IEnumerable<ConstructorInfo> GetConstructors()
        {
            return new ConstructorEnumerator(
                this,
                BaseReflectionHandle.GetMembers(MemberTypes.Constructor)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ConstructorInfo? GetConstructor(bool throwIfNotFound)
        {
            if (cachedCtorKeys.TryGetValue(this, out var ctorKey)
                &&
                CachedMembers.TryGetConstructor(ctorKey, out var ctor))
            {
                return ctor;
            }

            if (GetConstructors().SingleOrDefault().IsNot(out ctor))
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException($"Cannot find any constructor by {nameof(ReflectionMethodHandle)}: {this}");

                return ctor;
            }

            if (BaseReflectionHandle.CacheResults)
                CacheConstructor(ctor);

            return ctor;
        }

        public readonly bool Equals(ReflectionMethodHandle other)
        {
            return BaseReflectionHandle == other.BaseReflectionHandle
                   &&
                   ParameterKeys == other.ParameterKeys
                   &&
                   ReturnTypeFilter == other.ReturnTypeFilter;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReflectionMethodHandle typed && Equals(typed);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(BaseReflectionHandle)}: {BaseReflectionHandle}; {nameof(ParameterKeys)}: {ParameterKeys}; {nameof(ReturnTypeFilter)}: {ReturnTypeFilter})";
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(BaseReflectionHandle, ParameterKeys, ReturnTypeFilter);

            return hashCode.Value;
        }

        private readonly void CacheMethod(MethodInfo method)
        {
            if (cachedMethodKeys.TryAdd(this, new MethodKey(method), out var entry))
                entry.ExpirationTimeRelativeToNow = BaseReflectionHandle.GetCacheExpirationTimeRelativeToNowOrDefault();

            CachedMembers.TryAddMethod(method);
        }

        private readonly void CacheConstructor(ConstructorInfo ctor)
        {
            if (cachedCtorKeys.TryAdd(this, new MethodKey(ctor), out var entry))
                entry.ExpirationTimeRelativeToNow = BaseReflectionHandle.GetCacheExpirationTimeRelativeToNowOrDefault();

            CachedMembers.TryAddConstructor(ctor);
        }

        public struct MethodsEnumerator : IEnumerator<MethodInfo?>, IEnumerable<MethodInfo>
        {
            private readonly ReflectionMethodHandle reflectionHandle;

            private readonly IEnumerator<MemberInfo> sourceEnumerator;

            public MethodInfo? Current { get; private set; }

            readonly object? IEnumerator.Current => Current;

            public MethodsEnumerator(
                ReflectionMethodHandle reflectionHandle,
                IEnumerable<MemberInfo> source
                )
                :
                this()
            {
                CC.Guard.IsNotNull(source, nameof(source));

                this.reflectionHandle = reflectionHandle;
                sourceEnumerator = source.GetEnumerator();
            }

            public bool MoveNext()
            {
                while (sourceEnumerator.TryMoveNext(out var tCurrent))
                {
                    if (tCurrent is not MethodInfo method)
                        continue;

                    if (!reflectionHandle.IsReturnTypeMatch(method.ReturnType))
                        continue;

                    Current = method;

                    return true;
                }

                return false;
            }

            public readonly IEnumerator<MethodInfo> GetEnumerator() => this;

            readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            readonly void IEnumerator.Reset()
            {
                throw new NotSupportedException("Not resetable");
            }

            readonly void IDisposable.Dispose()
            {
            }
        }

        public struct ConstructorEnumerator : IEnumerator<ConstructorInfo?>, IEnumerable<ConstructorInfo>
        {
            private readonly ReflectionMethodHandle reflectionHandle;

            private readonly IEnumerator<MemberInfo> sourceEnumerator;

            public ConstructorInfo? Current { get; private set; }

            readonly object? IEnumerator.Current => Current;

            public ConstructorEnumerator(
                ReflectionMethodHandle reflectionHandle,
                IEnumerable<MemberInfo> source
                )
                :
                this()
            {
                CC.Guard.IsNotNull(source, nameof(source));

                this.reflectionHandle = reflectionHandle;
                sourceEnumerator = source.GetEnumerator();
            }

            public bool MoveNext()
            {
                while (sourceEnumerator.TryMoveNext(out var tCurrent))
                {
                    if (tCurrent is not ConstructorInfo ctor)
                        continue;

                    Current = ctor;

                    return true;
                }

                return false;
            }

            public readonly IEnumerator<ConstructorInfo> GetEnumerator() => this;

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
