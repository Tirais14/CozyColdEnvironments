using System.Text;
using CCEnvs.Patterns.Factories;

#nullable enable
namespace CCEnvs.Pools
{
    public class StringBuilderPool : ObjectPool<StringBuilder>
    {
        public static StringBuilderPool Shared { get; } = new();

        public StringBuilderPool(
            int capacity = 4,
            int? maxSize = null)
            :
            base(factory: Factory.Create(static () => new StringBuilder()),
                capacity: capacity,
                maxSize: maxSize)
        {

        }

        protected override void OnReturn(StringBuilder obj)
        {
            base.OnReturn(obj);
            obj.Clear();
        }
    }
}
