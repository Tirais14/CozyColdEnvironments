#nullable enable
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

using System;
using UnityEngine;

namespace UTIRLib.Diagnostics
{
    /// <summary>
    /// !Editor only
    /// </summary>
    public sealed class GizmosDrawer : MonoBehaviour
    {
        private static GizmosDrawer? instanceInternal;

        private static GizmosDrawer instance {
            get {
                if (instanceInternal == null)
                {
                    GameObject go = new(nameof(GizmosDrawer), typeof(GizmosDrawer)){
                        isStatic = true
                    };

                    instanceInternal = go.GetComponent<GizmosDrawer>();

                    DontDestroyOnLoad(go);
                }

                return instanceInternal;
            }
        }

        /// <summary>
        /// !Editor only
        /// </summary>
        public static event Action? onDrawGizmos;

        private void OnDrawGizmos() => onDrawGizmos?.Invoke();
    }
}