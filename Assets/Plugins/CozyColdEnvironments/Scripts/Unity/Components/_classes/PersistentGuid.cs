using CCEnvs.Collections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Components
{
    [Serializable]
    [DisallowMultipleComponent]
    public sealed class PersistentGuid : CCBehaviour
    {
        public static bool IgnoreWarnings { get; set; }
        private readonly static HashSet<string> guids = new();
        private static bool sceneSubscribed;

        [NonSerialized]
        private int pressCount;

        [field: SerializeField]
        [Tooltip("Serialized by inspector only for restoring value only. For exapmle from Json serialized value. Don't set the id manually.")]
        public string? Guid { get; private set; }

        [field: SerializeField]
        public int PressCountToRegenerate { get; set; } = 6;

        private void Reset()
        {
            InitializeOnLoad();
            Validate();

            if (Guid.IsNullOrWhiteSpace())
                GenerateGuid();
        }

        [InitializeOnLoadMethod]
        public static void InitializeOnLoad()
        {
            if (sceneSubscribed)
                return;

            SceneManager.activeSceneChanged += OnSceneChanged;
            sceneSubscribed = true;
        }

        private static void OnSceneChanged(Scene _, Scene __)
        {
            guids.Clear();
            guids.AddRange(GetSceneGuids());
        }

        private static HashSet<string> GetSceneGuids()
        {
            var cmps = GameObjectQuery.Scene.Components<PersistentGuid>().ToArray();
            var results = new HashSet<string>(cmps.Length);

            foreach (var cmp in cmps)
            {
                if (cmp.Guid.IsNullOrWhiteSpace())
                    continue;

                if (!results.Add(cmp.Guid))
                    throw new InvalidOperationException($"Found dublicate id '{cmp.Guid}'");
            }

            return results;
        }

        public async void GenerateGuid()
        {
            if ((!Application.isEditor || Application.isPlaying) && Guid.IsNotNullOrWhiteSpace())
                throw new InvalidOperationException("Cannot regenerate GameObject GUID in runtime and build");

            pressCount++;
            if (Guid.IsNotNullOrWhiteSpace()
                &&
                pressCount < PressCountToRegenerate)
            {
                return;
            }

            Guid = System.Guid.NewGuid().ToString();
            while (guids.Contains(Guid))
                Guid = System.Guid.NewGuid().ToString();

            pressCount = 0;
            guids.Add(Guid);
        }

        private void Validate()
        {
            if ((!Application.isEditor || Application.isPlaying) && !IgnoreWarnings)
                this.PrintWarning($"Using in runtime and not in editor does not make sense. Use '{typeof(RuntimeId)}' instead");

            if (Guid.IsNotNullOrWhiteSpace() && guids.Contains(Guid))
            {
                this.PrintError($"Found duplicate id. More often caused by instantiating {nameof(GameObject)} with {nameof(PersistentGuid)}. Guid will be destroyed");
                Destroy(this);
            }
        }
    }
}
