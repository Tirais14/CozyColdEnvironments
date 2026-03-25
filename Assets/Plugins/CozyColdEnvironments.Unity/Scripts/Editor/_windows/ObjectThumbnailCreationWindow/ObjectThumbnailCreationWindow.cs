using CCEnvs;
using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

#nullable enable
public class ObjectThumbnailCreationWindow : EditorWindow
{
    #region Serialized
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default!;

    [SerializeField]
    private Texture2D previewTexture;

    [SerializeField]
    private Object? selectedAsset;
    #endregion Serialized

    #region View
    private ListView assetsView = null!;

    private Button refreshButton = null!;

    private Label assetNameView = null!;

    private TextField typeNameView = null!;

    private VisualElement contentContainer = null!;

    private Image previewImage = null!;

    private Toggle isComponentToggle = null!;

    private Vector3Field positionOffsetView = null!;
    private Vector3Field rotationOffsetView = null!;
    #endregion View

    #region Data
    private List<Object> assets = new();

    private int previewSize = 512;

    private Scene previewScene;

    private GameObject? cameraObj;

    private Camera? sceneCamera;

    private Object? previewAsset;
    #endregion Data

    [MenuItem(EditorHelper.WINDOWS_TAB_NAME + "/" + EditorHelper.CCENVS_TAB + "/Thumbnail Creator")]
    public static void ShowExample()
    {
        ObjectThumbnailCreationWindow wnd = GetWindow<ObjectThumbnailCreationWindow>("Asset Thumbnail Creator");
        wnd.titleContent = new GUIContent("Asset Thumbnail Creator");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        ResolveContentContainer();
        ResolveAssetsView();
        ResolveRefreshButton();
        ResolvePreviewImage();
        ResolvePositionOffsetView();
        ResolveRotationOffsetView();
        ResolveTypeNameView();
        ResolveIsComponentToggle();
        FindAssets();
    }

    #region ResolveViews
    private void ResolveContentContainer()
    {
        contentContainer = rootVisualElement.Q("Content");
        contentContainer.dataSource = this;
    }

    private void ResolveAssetsView()
    {
        assetsView = rootVisualElement.Q<ListView>("Assets");
        assetsView.itemsSource = assets;
        assetsView.selectionChanged += OnSelectItem;
        assetsView.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
    }

    private void ResolveRefreshButton()
    {
        refreshButton = rootVisualElement.Q<Button>("RefreshButton");
        refreshButton.clicked += FindAssets;
    }

    private void ResolvePreviewImage()
    {
        previewImage = contentContainer.Q<Image>("Preview");
        previewImage.dataSource = this;
    }

    private void ResolveTypeNameView()
    {
        typeNameView = rootVisualElement.Q<TextField>("TypeName");
        typeNameView.RegisterValueChangedCallback(OnTypeNameChanged);
    }

    private void ResolvePositionOffsetView()
    {
        positionOffsetView = rootVisualElement.Q<Vector3Field>("PositionOffset");
        positionOffsetView.RegisterValueChangedCallback(OnPositionOffsetChanged);
    }

    private void ResolveRotationOffsetView()
    {
        rotationOffsetView = rootVisualElement.Q<Vector3Field>("RotationOffset");
        rotationOffsetView.RegisterValueChangedCallback(OnRotationOffsetChanged);
    }

    private void ResolveIsComponentToggle()
    {
        isComponentToggle = contentContainer.Q<Toggle>("IsComponent");
        isComponentToggle.RegisterValueChangedCallback(OnIsComponentToggleChanged);
    }
    #endregion ResolveViews

    #region Callbacks
    private void OnTypeNameChanged(ChangeEvent<string> ev)
    {
        FindAssets();
    }

    private void OnPositionOffsetChanged(ChangeEvent<Vector3> ev)
    {
        if (previewAsset == null)
            return;

        ApplyPositionOffset();
        RenderPreviewScene();
    }

    private void OnRotationOffsetChanged(ChangeEvent<Vector3> ev)
    {
        if (previewAsset == null)
            return;

        ApplyRotationOffset();
        RenderPreviewScene();
    }

    private void OnSelectItem(object item)
    {
        selectedAsset = assets[assetsView.selectedIndex];
        ResolvePreviewScene();
        ResolveCamera();
        ResolvePreviewAsset();
        RenderPreviewScene();
    }

    private void OnIsComponentToggleChanged(ChangeEvent<bool> ev)
    {
        FindAssets();
    }
    #endregion Callbacks

    private void FindAssets()
    {
        assets.Clear();

        string assetFilter = typeNameView.value.IsNullOrWhiteSpace() || isComponentToggle.value ? "GameObject" : typeNameView.value;

        var dbAssets =
            from guid in AssetDatabase.FindAssets($"t:{assetFilter}")
            where guid is not null
            select AssetDatabase.GUIDToAssetPath(guid) into path
            where path is not null
            select AssetDatabase.LoadAssetAtPath<Object>(path) into asset
            where asset != null && asset.GetType() != typeof(MonoScript)
            select asset;

        if (isComponentToggle.value)
        {
            dbAssets =
                from prefab in dbAssets.OfType<GameObject>()
                select prefab.GetComponents<Component>() into cmps
                from cmp in cmps
                let cmpType = cmp.GetType()
                where cmpType.FullName.StartsWith(typeNameView.value) || cmpType.Name.StartsWith(typeNameView.value)
                select cmp;
        }

        foreach (var asset in dbAssets)
            assets.Add(asset);

        assetsView.RefreshItems();
    }

    private void ApplyPositionOffset()
    {
        if (previewAsset == null)
            return;

        GetPreviewAssetTransform().Map(tr => tr.position = positionOffsetView.value);
    }

    private void ApplyRotationOffset()
    {
        if (previewAsset == null)
            return;

        GetPreviewAssetTransform().Map(tr => tr.rotation = Quaternion.Euler(rotationOffsetView.value));
    }

    private Maybe<Transform> GetPreviewAssetTransform()
    {
        if (previewAsset.Is<GameObject>(out var go))
            return go.transform;
        else if (previewAsset.Is<Component>(out var cmp))
            return cmp.transform;
        else
            return null;
    }

    private void ResolvePreviewScene()
    {
        if (!previewScene.IsValid())
            previewScene = EditorSceneManager.NewPreviewScene();
    }

    private void ResolveCamera()
    {
        if (cameraObj != null)
            return;

        cameraObj = new GameObject("Camera");
        cameraObj.transform.SetPositionAndRotation(new Vector3(0f, 0f, -10), Quaternion.identity);

        sceneCamera = cameraObj.AddComponent<Camera>();
        sceneCamera.aspect = 1f;
        sceneCamera.backgroundColor = Color.black;
        sceneCamera.clearFlags = CameraClearFlags.SolidColor;

        sceneCamera.targetTexture = new RenderTexture(
            previewSize,
            previewSize,
            32,
            RenderTextureFormat.ARGBFloat
            );

        SceneManager.MoveGameObjectToScene(cameraObj, previewScene);

        sceneCamera.scene = previewScene;
    }

    private void ResolvePreviewAsset()
    {
        if (previewAsset != null)
            DestroyImmediate(previewAsset);

        try
        {
            GameObject? prefab;

            if (previewAsset.IsNot<GameObject>(out prefab))
                if (previewAsset.Is<Component>(out var cmp))
                    prefab = cmp.gameObject;

            if (prefab == null)
                return;

            previewAsset = PrefabUtility.InstantiatePrefab(prefab, previewScene);
        }
        catch (System.Exception ex)
        {
            this.PrintException(ex);
        }

        ApplyPositionOffset();
        ApplyRotationOffset();
    }

    private void RenderPreviewScene()
    {
        if (sceneCamera == null)
            return;

        sceneCamera.Render();

        if (previewTexture == null)
        {
            previewTexture = new Texture2D(
                previewSize,
                previewSize,
                TextureFormat.RGBAFloat,
                false
                );
        }

        RenderTexture.active = sceneCamera.targetTexture;

        previewTexture.ReadPixels(new Rect(0f, 0f, previewSize, previewSize), 0, 0);
        previewTexture.Apply();

        RenderTexture.active = null;    
    }
}
