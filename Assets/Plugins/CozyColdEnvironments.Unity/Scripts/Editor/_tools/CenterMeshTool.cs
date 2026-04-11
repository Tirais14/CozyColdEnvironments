using CCEnvs.TypeMatching;
using UnityEditor;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Editr
{
    public static class CenterMeshTool
    {
        [MenuItem(EditorHelper.TOOLS_TAB_NAME + "/" + EditorHelper.CCENVS_TAB + "/Center Selected Object")]
        public static void CenterMesh()
        {
            if (Selection.activeObject == null)
                return;

            Transform objTransform;
            if (Selection.activeObject.Is<GameObject>(out var go))
                objTransform = go.transform;
            else if (Selection.activeObject.Is<Component>(out var cmp))
                objTransform = cmp.transform;
            else return;

            Undo.RegisterCompleteObjectUndo(Selection.activeObject, "Center Mesh");

            foreach (var meshFilter in objTransform.GetComponentsInChildren<MeshFilter>())
                meshFilter.transform.position = meshFilter.transform.position.WithX(0f).WithZ(0f);
        }
    }
}
