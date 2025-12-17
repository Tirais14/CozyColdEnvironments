using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public sealed class GameObjectSnapshot : Snapshot<GameObject>
    {
        [JsonInclude]
        [SerializeField]
        public string? name { get; set; }

        [JsonInclude]
        [SerializeField]
        public string? tag { get; set; }

        [JsonInclude]
        [SerializeField]
        public int layer { get; set; }

        [JsonInclude]
        [SerializeField]
        public bool activeSelf { get; set; } = true;

        [JsonInclude]
        [SerializeField]
        public TransformSnapshot? transform { get; set; }

        public GameObjectSnapshot()
        {
        }

        public GameObjectSnapshot(GameObject target) : base(target)
        {
            name = target.name;
            tag = target.tag;
            layer = target.layer;
            activeSelf = target.activeSelf;
            transform = target.transform.CaptureState();
        }

        public override GameObject Restore(GameObject target)
        {
            CC.Guard.IsNotNullTarget(target);
            CC.Guard.IsNotNull(transform, nameof(target));
            Guard.IsNotNullOrWhiteSpace(name);

            target.name = name;
            target.tag = tag;
            target.layer = layer;
            target.SetActive(activeSelf);
            transform.Restore(target.transform);

            return target;
        }
    }

    public static class GameObjectSnapshotExtensions
    {
        public static GameObjectSnapshot CaptureState(this GameObject soucre)
        {
            CC.Guard.IsNotNullSource(soucre);
            return new GameObjectSnapshot(soucre);
        }
    }
}
