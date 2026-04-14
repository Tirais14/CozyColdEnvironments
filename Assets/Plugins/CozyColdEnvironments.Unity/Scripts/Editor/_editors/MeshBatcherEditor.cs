#if UNITY_EDITOR
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Components.Specialized;
using Humanizer;
using UnityEditor;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.Editr
{
    [CustomEditor(typeof(MeshBatcher))]
    public sealed class MeshBatcherEditor : CCEditor
    {
        private Button batchButton = null!;

        private Button restoreButton = null!;

        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();

            batchButton = new Button(OnBatchButton)
            {
                text = nameof(MeshBatcher.BatchMeshFilters).Humanize()
            };
            restoreButton = new Button(OnRestoreButton)
            {
                text = nameof(MeshBatcher.RestoreMeshFilters).Humanize()
            };

            root.Add(batchButton);
            root.Add(restoreButton);

            return root;
        }

        private void OnBatchButton()
        {
            if (target.IsNot<MeshBatcher>(out var meshBatcher))
                return;

            Undo.RecordObject(meshBatcher, nameof(MeshBatcher.BatchMeshFilters).Humanize());
            meshBatcher.BatchMeshFiltersCore();
        }

        private void OnRestoreButton()
        {
            if (target.IsNot<MeshBatcher>(out var meshBatcher))
                return;

            Undo.RecordObject(meshBatcher, nameof(meshBatcher.RestoreMeshFilters).Humanize());
            meshBatcher.RestoreMeshFiltersCore();
        }
    }
}
#endif