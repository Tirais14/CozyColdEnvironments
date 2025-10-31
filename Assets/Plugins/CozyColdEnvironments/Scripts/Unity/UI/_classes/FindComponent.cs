using CCEnvs.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity
{
    public class FindComponentInScene
    {
        public readonly static FindComponentInScene Q = new();

        public static FindComponentInScene Empty => new();

        private readonly static Type defaultType = typeof(Component);

        public FindObjectsInactive Inactive { get; protected set; } = FindObjectsInactive.Exclude;
        public FindObjectsSortMode SortMode { get; protected set; } = FindObjectsSortMode.None;

        public FindComponentInScene IncludeInactive()
        {
            Inactive = FindObjectsInactive.Include;

            return this;
        }

        public FindComponentInScene SortByInstanceID()
        {
            SortMode = FindObjectsSortMode.InstanceID;

            return this;
        }

        public FindComponentInScene Reset()
        {
            Inactive = FindObjectsInactive.Exclude;
            SortMode = FindObjectsSortMode.None;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<object> Components(Type? type = null)
        {
            type ??= defaultType;

            bool anyType = type == defaultType;

            IEnumerable<object> results;

            if (type.IsType(defaultType))
            {
                results = Object.FindObjectsByType(type, Inactive, SortMode)
                                .Where(cmp => anyType || cmp.IsType(type));
            }
            else
            {
                results = Object.FindObjectsByType(defaultType, Inactive, SortMode)
                                .Where(cmp => anyType || cmp.IsType(type));
            }

            Reset();

            return results;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Components<T>()
        {
            return Components(typeof(T)).Cast<T>();
        }
    }
}
