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

        [SerializeField]
        private bool startAtRuntime;

        public GameObject Source {
            get => source;
            set => source = value;  
        }

        public LODGroup LODGroup {
            get => lodGroup;
            set => lodGroup = value;
        }

        public int LODIndex {
            get => lodIndex;
            set => lodIndex = value;
        }

        public bool StartAtRuntime {
            get => startAtRuntime;
            set => startAtRuntime = value;
        }

        protected override void Start()
        {
            base.Start();

            if (startAtRuntime || !Application.isPlaying)
                Execute();

            Destroy(this);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
                return;

            if (Time.frameCount % 30 != 0)
                return;

            Execute();
        }
#endif

        public void Execute()
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
