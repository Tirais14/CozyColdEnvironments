using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public class CompositeComponent<T, T1> : ICompositeComponent<T, T1>
        where T : Component
        where T1 : Component
    {
        public T Part0 { get; }
        public T1 Part1 { get; }

        public CompositeComponent(T part0, T1 part1)
        {
            Part0 = part0;
            Part1 = part1;
        }

        public IEnumerator<Component> GetEnumerator()
        {
            yield return Part0;
            yield return Part1;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
