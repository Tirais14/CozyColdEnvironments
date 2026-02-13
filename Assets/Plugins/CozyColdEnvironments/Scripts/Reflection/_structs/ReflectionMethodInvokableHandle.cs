#nullable enable
using CCEnvs.Reflection.Caching;
using System;
using System.Runtime.CompilerServices;

namespace CCEnvs.Reflection
{
    public struct ReflectionMethodInvokableHandle
    {
        //private Delegate? methodFunc;

        public ReflectionMethodHandle MethodReflectionHandle { readonly get; init; }

        public object? Target { readonly get; init; }

        public ReflectionMethodInvokableHandle(ReflectionMethodHandle methodCore)
            :
            this()
        {
            MethodReflectionHandle = methodCore; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReflectionMethodInvokableHandle WithMethodReflectionHandle(
            ReflectionMethodHandle methodReflectionHandle
            )
        {
            return new ReflectionMethodInvokableHandle(methodReflectionHandle)
            {
                Target = Target
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReflectionMethodInvokableHandle WithTarget(object? target)
        {
            return new ReflectionMethodInvokableHandle(MethodReflectionHandle)
            {
                Target = target
            };
        }

        //public object? Invoke(params object[] args)
        //{
        //    var method = MethodReflectionHandle.GetMethod();

        //    method.CreateDelegate()
        //    if (CachedMembers.TryGetMethodDelegate(method, out methodFunc))
        //        return methodFunc(args);
        //}
    }
}
