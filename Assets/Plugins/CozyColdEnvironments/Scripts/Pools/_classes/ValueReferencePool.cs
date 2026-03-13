using CCEnvs.Patterns.Factories;

#nullable enable
namespace CCEnvs.Pools
{
    public class ValueReferencePool<T> : ObjectPool<ValueReference<T>>
    {
        public static ValueReferencePool<T> Shared { get; } = new();

        public ValueReferencePool(
            int capacity = 2,
            int? maxSize = null
            )
            :
            base(
                Factory.Create(() => new ValueReference<T>()),
                capacity: capacity,
                maxSize: maxSize)
        {

        }

        protected override void OnReturn(ValueReference<T> obj)
        {
            base.OnReturn(obj);
            obj.Value = default!;
        }
    }
}
