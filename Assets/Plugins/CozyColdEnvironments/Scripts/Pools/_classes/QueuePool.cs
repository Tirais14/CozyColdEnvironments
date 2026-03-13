using CCEnvs.Patterns.Factories;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Pools
{
    public class QueuePool<T> : ObjectPool<Queue<T>>
    {
        public static QueuePool<T> Shared { get; } = new();

        public QueuePool(
            int capacity = 1,
            int? maxSize = null
            )
            :
            base(
                Factory.Create(() => new Queue<T>()),
                capacity: capacity,
                maxSize: maxSize)
        {

        }

        protected override void OnReturn(Queue<T> obj)
        {
            base.OnReturn(obj);
            obj.Clear();
        }
    }
}
