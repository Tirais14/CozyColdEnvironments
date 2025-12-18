using CCEnvs.Collections;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    [DisallowMultipleComponent]
    public sealed class PersistentGuid : CCBehaviour
    {
        public static bool IgnoreWarnings { get; set; }
        private readonly static HashSet<string> guids = new();
        private static bool sceneSubscribed;

        [JsonProperty]
        [field: SerializeField]
        [Tooltip("Serialized by inspector only for restoring value only. For exapmle from Json serialized value. Don't set the id manually.")]
        public string? Guid { get; private set; }

        [NonSerialized]
        private int pressCount;

        [field: SerializeField]
        public int PressCountToRegenerate { get; set; } = 6;

        private void Reset()
        {
            if (Guid.IsNullOrWhiteSpace())
                GenerateGuid();
        }

        protected override void Awake()
        {
            base.Awake();

            SubscribeOnSceneChanged();

            if ((!Application.isEditor || Application.isPlaying) && !IgnoreWarnings)
                this.PrintWarning($"Using in runtime and not in editor does not make sense. Use '{typeof(RuntimeId)}' instead");
        }

        [InitializeOnLoadMethod]
        public static void SubscribeOnSceneChanged()
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
    }
}
