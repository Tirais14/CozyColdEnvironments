using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection
{
    public struct ReflectedMethod
    {
        public Reflected Core { readonly get; init; }

        public StructuralArray<ParameterKey> ParameterKeys { readonly get; init; }

        public Type? ReturnType { readonly get; init; }

        public ReflectedMethod Create()
        {
            return new ReflectedMethod
            {
                ParameterKeys = StructuralArray<ParameterKey>.Empty
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethod WithCore(Reflected core)
        {
            return new ReflectedMethod
            {
                Core = core,
                ParameterKeys = ParameterKeys,
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethod WithParameterKeys(params ParameterKey[] paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectedMethod
            {
                Core = Core,
                ParameterKeys = paramKeys.ToStructuralArray(),
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethod WithParameterKeys(IEnumerable<ParameterKey> paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectedMethod
            {
                Core = Core,
                ParameterKeys = paramKeys.ToStructuralArray(),
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethod WithParameterKeys(StructuralArray<ParameterKey> paramKeys)
        {
            Guard.IsNotNull(paramKeys, nameof(paramKeys));

            return new ReflectedMethod
            {
                Core = Core,
                ParameterKeys = paramKeys,
                ReturnType = ReturnType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethod WithReturnType(Type? returnType)
        {
            return new ReflectedMethod
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
        public readonly MethodInfo GetMethod()
        {
            return GetMethods().Single();
        }

        public struct MethodsEnumerator : IEnumerator<MethodBase?>, IEnumerable<MethodBase>
        {
            private readonly ReflectedMethod reflectedHandle;

            private readonly IEnumerator<MethodBase> sourceEnumerator;

            public MethodBase? Current { get; private set; }

            readonly object? IEnumerator.Current => Current;

            public MethodsEnumerator(ReflectedMethod handle, IEnumerable<MethodBase> source)
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

            void IEnumerator.Reset()
            {
                throw new NotSupportedException("Not resetable");
            }

            void IDisposable.Dispose()
            {
            }
        }
    }
}
