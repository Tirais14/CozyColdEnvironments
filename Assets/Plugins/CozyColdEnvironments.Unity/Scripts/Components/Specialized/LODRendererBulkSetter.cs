using CCEnvs.Collections;
using CCEnvs.Unity.Components;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Editr
{
    [ExecuteInEditMode]
    public sealed class LODRendererBulkSetter : CCBehaviour
    {
        [SerializeField]
        private GameObject source;

        [SerializeField]
        private LODGroup lodGroup;

        [SerializeField]
        private int lodIndex = -1;

        protected override void Start()
        {
            base.Start();

            Execute();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
                return;

            if (Time.frameCount % 30 != 0)
                return;

            Execute();
        }
#endif

        private void Execute()
        {
            if (source == null || lodIndex < 0)
                return;

            var renderers = source.GetComponentsInChildren<MeshRenderer>(includeInactive: true);

            var lods = lodGroup.GetLODs();

            if (lodIndex >= lods.Length)
                return;

            LOD lod = lods[lodIndex];

            lods[lodIndex] = new LOD(lod.screenRelativeTransitionHeight, renderers.Concat(lod.renderers).Distinct().ToArray());

            lodGroup.SetLODs(lods);
        }
    }
}
