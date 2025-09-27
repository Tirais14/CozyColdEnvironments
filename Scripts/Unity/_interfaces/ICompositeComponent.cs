using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public interface ICompositeComponent : IEnumerable<Component>
    {
    }
    public interface ICompositeComponent<out T, out T1>
        : ICompositeComponent
        where T : Component
        where T1 : Component
    {
        T Part0 { get; }
        T1 Part1 { get; }
    }
    public interface ICompositeComponent<out T, out T1, out T2>
        : ICompositeComponent<T, T1>
        where T : Component
        where T1 : Component
        where T2 : Component
    {
        T2 Part2 { get; }
    }
    public interface ICompositeComponent<out T, out T1, out T2, out T3>
        : ICompositeComponent<T, T1, T2>
        where T : Component
        where T1 : Component
        where T2 : Component
        where T3 : Component
    {
        T3 Part3 { get; }
    }
}
