using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    public sealed class GameObjectExtraInfo : IGameObjectExtraInfo
    {
        [field: SerializeField]
        public string? PersistenGuid { get; private set; }

        [field: SerializeField]
        public string? RuntimeId { get; private set; }

        [field: SerializeField]
        public HierarchyPath HierarchyPath { get; private set; }

        public GameObjectExtraInfo(GameObject gameObject)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            PersistenGuid = gameObject.GetPersistentGuid().Raw;
            RuntimeId = gameObject.GetRuntimeId().Raw;
            HierarchyPath = gameObject.GetHierarchyPath();
        }

        [JsonConstructor]
        public GameObjectExtraInfo(string? persistenGuid, string? runtimeId, HierarchyPath hierarchyPath)
        {
            PersistenGuid = persistenGuid;
            RuntimeId = runtimeId;
            HierarchyPath = hierarchyPath;
        }

        public Maybe<GameObject> FindGameObject(bool includeInactive = false)
        {
            if (PersistenGuid.IsNotNullOrWhiteSpace()
                &&
                GameObjectHelper.FindByPersistenGuid(PersistenGuid, includeInactive).TryGetValue(out var go))
            {
                return go;
            }

            if (RuntimeId.IsNotNullOrWhiteSpace()
                &&
                GameObjectHelper.FindByRuntimeId(RuntimeId, includeInactive).TryGetValue(out go))
            {
                return go;
            }

            if (HierarchyPath.IsNotDefault()
                &&
                GameObjectHelper.FindByHierarchyPath(HierarchyPath, includeInactive).TryGetValue(out go))
            {
                return go;
            }

            return Maybe<GameObject>.None;
        }
    }

    public static class GameObjectExtraInfoExtensions
    {
        public static GameObjectExtraInfo GetExtraInfo(this GameObject source)
        {
            return new GameObjectExtraInfo(source);
        }

        public static GameObjectExtraInfo GetExtraInfo(this Component source)
        {
            return new GameObjectExtraInfo(source.gameObject);
        }
    }
}
