using UnityEngine;

#nullable enable
namespace UTIRLib
{
    public interface IProperty<T>
    {
        T Value { get; }
    }
}
