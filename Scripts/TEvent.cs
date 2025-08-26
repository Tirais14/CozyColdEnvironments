using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UTIRLib.Disposables;

#nullable enable
namespace UTIRLib
{
    public sealed class TEvent<T>
        where T : Delegate
    {
        private readonly List<T> actions = new();

        public TEvent(T function)
        {
            actions.Add(function);
        }

        public IDisposable Add(T function)
        {
            actions.Add(function);

            return new Subscription<T>((x) => Remove(x), function);
        }

        public void Remove(T function)
        {
            actions.Remove(function);
        }
    }
}
