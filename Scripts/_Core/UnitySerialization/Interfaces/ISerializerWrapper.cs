using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace UTIRLib.Unity.Serialization
{
    public interface ISerializerWrapper<T>
    {
        T Value { get; }
    }

    public static class ISerializerWrapperExtensions
    {
        public static T[] AsValueArray<TWrapper, T>(this IEnumerable<TWrapper> wrappers)
            where TWrapper : ISerializerWrapper<T>
        {
            return wrappers.Select(x => x.Value).ToArray();
        }
    }
}
