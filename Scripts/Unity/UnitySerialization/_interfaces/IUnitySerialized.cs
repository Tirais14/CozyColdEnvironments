using System.Collections.Generic;
using System.Linq;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.Unity.Serialization
{
    public interface IUnitySerialized
    {
        public bool IsDefault => ObjectValidator.EqaulsDefaultByFields(this);
    }
    public interface IUnitySerialized<T> : IUnitySerialized
    {
        T Value { get; }
    }

    public static class ISerializerWrapperExtensions
    {
        public static T[] AsValueArray<TWrapper, T>(this IEnumerable<TWrapper> wrappers)
            where TWrapper : IUnitySerialized<T>
        {
            return wrappers.Select(x => x.Value).ToArray();
        }
    }
}
