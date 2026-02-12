using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor.Toolbars;

#nullable enable
namespace CCEnvs.Reflection
{
    public struct ReflectedMethodHandle : IEquatable<ReflectedMethodHandle>
    {
        private int? hashCode;

        public ReflectedHandle Core { readonly get; init; }

        public StructuralArray<ParameterKey> ParameterKeys { readonly get; init; }

        public Type? ReturnType { readonly get; init; }

        public static bool operator ==(ReflectedMethodHandle left, ReflectedMethodHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReflectedMethodHandle left, ReflectedMethodHandle right)
        {
            return !(left == right);
        }

        public ReflectedMethodHandle Create()
        {
            return new ReflectedMethodHandle
            {
                ParameterKeys = StructuralArray<ParameterKey>.Empty
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethodHandle WithCore(ReflectedHandle core)
        {
            return new ReflectedMethodHandle
            {
                Core = core,
                ParameterKeys = ParameterKeys,
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethodHandle WithParameterKeys(params ParameterKey[] paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectedMethodHandle
            {
                Core = Core,
                ParameterKeys = paramKeys.ToStructuralArray(),
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethodHandle WithParameterKeys(IEnumerable<ParameterKey> paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectedMethodHandle
            {
                Core = Core,
                ParameterKeys = paramKeys.ToStructuralArray(),
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethodHandle WithParameterKeys(StructuralArray<ParameterKey> paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectedMethodHandle
            {
                Core = Core,
                ParameterKeys = paramKeys,
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethodHandle WithReturnType(Type? returnType)
        {
            return new ReflectedMethodHandle
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
            if (!GetMethods().SingleOrDefault().Let(out var result))
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException($"Handle {this} not found any member");

                return result;
            }

            return result;
        }

        public readonly bool Equals(ReflectedMethodHandle other)
        {
            return Core == other.Core
                   &&
                   ParameterKeys == other.ParameterKeys
                   &&
                   ReturnType == other.ReturnType;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReflectedMethodHandle typed && Equals(typed);
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

        public struct MethodsEnumerator : IEnumerator<MethodBase?>, IEnumerable<MethodBase>
        {
            private readonly ReflectedMethodHandle reflectedHandle;

            private readonly IEnumerator<MethodBase> sourceEnumerator;

            public MethodBase? Current { get; private set; }

            readonly object? IEnumerator.Current => Current;

            public MethodsEnumerator(
                ReflectedMethodHandle handle,
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
