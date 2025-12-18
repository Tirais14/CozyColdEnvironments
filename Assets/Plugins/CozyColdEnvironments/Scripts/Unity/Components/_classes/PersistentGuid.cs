using CCEnvs.Collections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
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
            SubscribeOnActiveScene();
            Validate();

            if (Guid.IsNullOrWhiteSpace())
                GenerateGuid();
        }

        private void OnValidate()
        {
            Validate();
        }

        private void SubscribeOnActiveScene()
        {
            if (sceneSubscribed)
                return;

            SceneManager.activeSceneChanged += OnSceneChanged;
            sceneSubscribed = true;
        }

        private void OnSceneChanged(Scene _, Scene __)
        {
            guids.Clear();
            guids.AddRange(GetSceneGuids());
        }

        private HashSet<string> GetSceneGuids()
        {
            var cmps = GameObjectQuery.Scene.Components<PersistentGuid>().ToArray();
            var results = new HashSet<string>(cmps.Length);

            foreach (var cmp in cmps)
            {
                if (cmp.Guid.IsNullOrWhiteSpace())
                    continue;

                if (cmp == this)
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

            if (!Application.isPlaying)
            {
                guids.Clear();
                guids.AddRange(GetSceneGuids());
            }

            Guid = System.Guid.NewGuid().ToString();
            while (guids.Contains(Guid))
                Guid = System.Guid.NewGuid().ToString();

            pressCount = 0;
            guids.Add(Guid);

            if (!Application.isPlaying)
                guids.Clear();
        }

        private void Validate()
        {
            if (!Application.isPlaying)
            {
                SubscribeOnActiveScene();
                guids.Clear();
                guids.AddRange(GetSceneGuids());
            }

            if ((Application.isPlaying) && !IgnoreWarnings)
                this.PrintWarning($"Using in runtime and not in editor does not make sense. Use '{typeof(RuntimeId)}' instead");

            if (Guid.IsNotNullOrWhiteSpace() && guids.Contains(Guid))
            {
                this.PrintError($"Found duplicate id. More often caused by instantiating {nameof(GameObject)} with {nameof(PersistentGuid)}. Guid will be destroyed");

                if (Application.isPlaying)
                    Destroy(this);
            }

            if (!Application.isPlaying)
                guids.Clear();
        }
    }
}
