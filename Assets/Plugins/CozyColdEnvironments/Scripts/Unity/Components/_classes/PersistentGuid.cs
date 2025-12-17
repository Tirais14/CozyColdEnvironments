using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    [DisallowMultipleComponent]
    public sealed class PersistentGuid : CCBehaviour
    {
        [JsonInclude]
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
                GenerateGuidAsync().AttachExternalCancellation(destroyCancellationToken).Forget();
        }

        public async UniTask GenerateGuidAsync()
        {
            if (Application.isPlaying && Guid.IsNotNullOrWhiteSpace())
                throw new System.InvalidOperationException("Cannot regenerate GameObject GUID in play mode");

            pressCount++;
            if (Guid.IsNotNullOrWhiteSpace()
                &&
                pressCount < PressCountToRegenerate)
            {
                return;
            }

            HashSet<string> sceneGuids;
            try
            {
                sceneGuids = await GetSceneGuidsAsync();
            }
            catch (Exception)
            {
                throw;
            }

            var guid = System.Guid.NewGuid();
            while (sceneGuids.Contains(guid.ToString()))
                guid = System.Guid.NewGuid();

            pressCount = 0;
            Guid = guid.ToString();
        }

        private async UniTask<HashSet<string>> GetSceneGuidsAsync()
        {
            var cmps = GameObjectQuery.Scene.Components<PersistentGuid>().ToArray();
            var results = new HashSet<string>(cmps.Length);

            if (Application.isPlaying)
                await UniTask.SwitchToThreadPool();

            foreach (var cmp in cmps)
            {
                if (cmp == this)
                    continue;

                if (cmp.Guid.IsNullOrWhiteSpace())
                {
                    cmp.PrintError("Missing guid");
                    continue;
                }

                if (!results.Add(cmp.Guid))
                    throw new InvalidOperationException($"Found dublicate id '{cmp.Guid}'");
            }

            if (Application.isPlaying)
                await UniTask.SwitchToMainThread();

            return results;
        }
    }
}
