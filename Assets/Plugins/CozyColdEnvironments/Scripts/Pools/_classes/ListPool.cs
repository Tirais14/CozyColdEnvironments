#nullable enable
using CCEnvs.Patterns.Factories;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

namespace CCEnvs.Pools
{
    public class ListPool<T> : ObjectPool<List<T>>
    {
        private static ListPool<T> Instance { get; } = new();

        public ListPool()
            :
            base(factory: Factory.Create(() => new List<T>()))
        {
            
        }

#if UNITY_2017_1_OR_NEWER
        public override PooledHandle<List<T>> Get()
        {
            var uHandle = UnityEngine.Pool.ListPool<T>.Get(out var list);

            return new PooledHandle<List<T>>(list, uHandle,
                static (list, uHandle) =>
                {
                    ((IDisposable)uHandle).Dispose();
                });
        }

        public override void Return(List<T> obj)
        {
            Guard.IsNotNull(obj, nameof(obj));
        }
#endif
    }
}
