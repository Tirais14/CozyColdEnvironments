using CCEnvs.Collections;
using CCEnvs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.Components
{
    [Serializable]
    [DisallowMultipleComponent]
    public sealed class PersistentGuid : CCBehaviour
    {
        public static bool IgnoreWarnings { get; set; }

        private readonly static HashSet<string> guids = new();

        private static bool sceneUnloadedSubscribed;

        [NonSerialized]
        private int pressCount;

        [field: SerializeField]
        [Tooltip("Serialized by inspector only for restoring value only. For exapmle from Json serialized value. Don't set the id manually.")]
        public string? Guid { get; private set; }

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

            if (Application.isPlaying && !IgnoreWarnings)
                this.PrintWarning($"Using in runtime and not in editor does not make sense. Use \"{typeof(RuntimeId)}\" instead");

            if (!sceneUnloadedSubscribed && Application.isPlaying)
                SubscibeToSceneUnloaded();
        }

        protected override void Start()
        {
            base.Start();

            if (Guid != null)
            {
                if (guids.Contains(Guid))
                    throw new InvalidOperationException("Found duplicate GUID");

                if (Guid.IsNotNullOrWhiteSpace())
                    guids.Add(Guid);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Guid.IsNotNullOrWhiteSpace())
                guids.Remove(Guid!);
        }

        private static void RemoveGuidByScene(Scene scene)
        {
            _ = scene.GetRootGameObjects()
                .AsValueEnumerable()
                .SelectMany(go => go.GetComponentsInChildren<PersistentGuid>(includeInactive: true))
                .Where(x => x.Guid != null)
                .Select(guidCmp => guids.Remove(guidCmp.Guid!))
                .ToArray();
        }

        private static void SubscibeToSceneUnloaded()
        {
            SceneManager.sceneUnloaded += RemoveGuidByScene;
            sceneUnloadedSubscribed = true;
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
                    throw new InvalidOperationException($"Found dublicate id \"{cmp.Guid}\"");
            }

            return results;
        }

        public void GenerateGuid()
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
    }
}
