#if UNITY_EDITOR
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CCEnvs.FuncLanguage;
using CCEnvs.UnityEditor;
using CommunityToolkit.Diagnostics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Editr
{
    [InitializeOnLoad]
    public sealed class PlaceObjectsBulkEditorWindow : CCEditorWindow
    {
        //private readonly List<VisualElement> objects = new();

        private ObjectField objectToPlace = null!;

        private ObjectField placingParent = null!;

        private Stopwatch lastPlacedObjectWatch = new();

        //private ListView objectsListView = null!;

        [MenuItem(EditorHelper.WINDOWS_TAB_NAME + "/" + EditorHelper.CCENVS_TAB + "/Place Objects Bulk")]
        public static EditorWindow Open()
        {
            return GetWindow<PlaceObjectsBulkEditorWindow>("Place Objects Bulk");
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneViewGUI;
            lastPlacedObjectWatch.Start();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui += OnSceneViewGUI;
            lastPlacedObjectWatch.Stop();
        }

        protected override void CreateElements()
        {
            objectToPlace = new ObjectField("To Place Object")
            {
                objectType = typeof(GameObject),
                allowSceneObjects = true,
            };

            placingParent = new ObjectField("Placing Parent")
            {
                objectType = typeof(Transform),
                allowSceneObjects = true,
            };

            //var objectListViewLabel = new Label("Objects");

            //root.Add(objectListViewLabel);

            //objectsListView = new ListView()
            //{
            //    name = "Objects",
            //    itemsSource = objects,
            //    allowAdd = true,
            //    allowRemove = true,
            //    makeItem = CreateObject,
            //    bindItem = BindObject,
            //    showAddRemoveFooter = true,
            //};
        }

        //private VisualElement CreateObject()
        //{
        //    var view = new VisualElement();

        //    var objField = new ObjectField()
        //    {
        //        objectType = typeof(GameObject),
        //        allowSceneObjects = true,
        //    };

        //    view.Add(objField);

        //    return view;
        //}

        //private void BindObject(VisualElement element, int index)
        //{

        //}

        private void CreateFields()
        {
            //surfaceObjField = new ObjectField("Surface")
            //{
            //    objectType = typeof(GameObject),
            //    allowSceneObjects = true,
            //};
        }

        private bool TryRaycast(
            Vector2 mousePos,
            out RaycastHit hit
            )
        {
            var ray = HandleUtility.GUIPointToWorldRay(mousePos);

            var hitBoxed = HandleUtility.RaySnap(ray);

            if (hitBoxed is not RaycastHit tHit)
            {
                hit = default;
                return false;
            }

            hit = tHit;
            return true;
        }

        private void PlaceObject()
        {
            if (Event.current == null)
                return;

            if (!TryRaycast(Event.current.mousePosition, out var raycastHit))
                return;

            GameObject? toPlaceObj = (GameObject?)PrefabUtility.InstantiatePrefab(objectToPlace.value);

            if (toPlaceObj == null)
                toPlaceObj = (GameObject?)Instantiate(objectToPlace.value);

            if (toPlaceObj == null)
                return;

            toPlaceObj.transform.rotation *= Quaternion.Euler(0f, UnityEngine.Random.Range(-180, 180), 0f);

            var r = UnityEngine.Random.Range(-0.15f, 0.15f);

            toPlaceObj.transform.localScale += new Vector3(r, r, r);

            Undo.RegisterCreatedObjectUndo(toPlaceObj, "Place Prefab");

            toPlaceObj.transform.position = raycastHit.point;

            lastPlacedObjectWatch.Restart();
        }

        private void OnSceneViewGUI(SceneView sceneView)
        {
            if (objectToPlace.value.IsNull())
                return;

            if (lastPlacedObjectWatch.Elapsed.Milliseconds < 256)
                return;

            if (Event.current.type != EventType.KeyDown
                ||
                Event.current.keyCode != KeyCode.Space)
            {
                return;
            }

            PlaceObject();
        }

    }
}
#endif