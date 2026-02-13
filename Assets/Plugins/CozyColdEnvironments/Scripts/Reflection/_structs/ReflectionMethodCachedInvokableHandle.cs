#nullable enable
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CCEnvs.Reflection
{
    public struct ReflectionMethodCachedInvokableHandle : IEquatable<ReflectionMethodCachedInvokableHandle>
    {
        private readonly MethodInfo method;

        private int? hashCode;

        public ReflectionMethodHandle MethodReflectionHandle { readonly get; init; }

        public object? Target { readonly get; init; }

        public ReflectionMethodCachedInvokableHandle(
            ReflectionMethodHandle methodCore,
            MethodInfo method
            )
            :
            this()
        {
            this.method = method;
            MethodReflectionHandle = methodCore; 
        }

        public static bool operator ==(ReflectionMethodCachedInvokableHandle left, ReflectionMethodCachedInvokableHandle right)
        {
            return left.Equals(right); 
        }

        public static bool operator !=(ReflectionMethodCachedInvokableHandle left, ReflectionMethodCachedInvokableHandle right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReflectionMethodCachedInvokableHandle WithMethodReflectionHandle(
            ReflectionMethodHandle methodReflectionHandle
            )
        {
            return new ReflectionMethodCachedInvokableHandle(methodReflectionHandle, method)
            {
                Target = Target,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReflectionMethodCachedInvokableHandle WithTarget(object? target)
        {
            return new ReflectionMethodCachedInvokableHandle(MethodReflectionHandle, method)
            {
                Target = target,
            };
        }

        //public object? Invoke(Type funcTypeIncludeSourceType, params object[] args)
        //{
        //    Guard.IsNotNull(funcTypeIncludeSourceType, nameof(funcTypeIncludeSourceType));
        //    Guard.IsNotNull(args, nameof(args));

        //    Delegate dlg;

        //    if (Target is not null)
        //    {
        //        dlg = method.CreateDelegate(funcTypeIncludeSourceType);
        //    }
        //}

        public readonly bool Equals(ReflectionMethodCachedInvokableHandle other)
        {
            return method == other.method
                   &&
                   MethodReflectionHandle == other.MethodReflectionHandle
                   &&
                   EqualityComparer<object?>.Default.Equals(Target, other.Target);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReflectionMethodCachedInvokableHandle typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(method, MethodReflectionHandle, Target);

            return hashCode.Value;
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(method)}: {method}; {nameof(MethodReflectionHandle)}: {MethodReflectionHandle}; {nameof(Target)}: {Target})";
        }
    }
}
