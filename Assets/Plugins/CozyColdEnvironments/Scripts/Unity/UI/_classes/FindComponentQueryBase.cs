using CCEnvs.Diagnostics;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public class FindComponentQueryBase<TThis> : IFindComponentQuery<TThis>
        where TThis : new()
    {
        public readonly static TThis Instance = new();

        public static TThis Empty => new();

        public GameObject Source { get; protected set; } = null!;
        public bool includeInactive { get; protected set; }
        public FindMode findMode { get; protected set; } = FindMode.Self;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TThis From(GameObject gameObject)
        {
            if (gameObject == null)
            {
                this.PrintError($"{nameof(gameObject)} is null.");
                return this.As<TThis>();
            }

            Source = gameObject;
            return this.As<TThis>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TThis From(Component component)
        {
            if (component == null)
            {
                this.PrintError($"{nameof(component)} is null.");
                return this.As<TThis>();
            }

            Source = component.gameObject;
            return this.As<TThis>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TThis IncludeInactive()
        {
            includeInactive = true;

            return this.As<TThis>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TThis InSelf()
        {
            findMode = FindMode.Self;

            return this.As<TThis>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TThis InChildren()
        {
            findMode = FindMode.InChilds;

            return this.As<TThis>();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TThis InParent()
        {
            findMode = FindMode.InParents;

            return this.As<TThis>();
        }

        public TThis Reset()
        {
            Source = null!;
            includeInactive = false;
            findMode = FindMode.Self;

            return this.As<TThis>();
        }
    }
}
