using System.Collections.Generic;
using System.Linq;
using CozyColdEnvironments.Reflection;

#nullable enable
namespace CozyColdEnvironments.Unity.Serialization
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
