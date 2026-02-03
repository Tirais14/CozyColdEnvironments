using CCEnvs.Patterns.Factories;
using System.Text;

#nullable enable
namespace CCEnvs.Pools
{
    public class StringBuilderPool : ObjectPool<StringBuilder>
    {
        public StringBuilderPool(
            int capacity = 4,
            int? maxSize = null)
            :
            base(factory: Factory.Create(static () => new StringBuilder()),
                capacity: capacity,
                maxSize: maxSize)
        {

        }
    }
}
