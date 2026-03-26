using CCEnvs;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CCEnvs.Unity;
using CCEnvs.Unity.Editr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

#nullable enable
public class ObjectThumbnailCreationWindow : EditorWindow
{
    public const string SAVE_PATH = nameof(ObjectThumbnailCreationWindow) + "/data.json";

    #region Serialized
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default!;

    [SerializeField]
    private Texture2D? previewTexture;

    [SerializeField]
    private SceneAsset? previewSceneAsset;

    [SerializeField]
    private Light? lightPrefab;

    [SerializeField]
    private GameObject? volumePrefab;

    [SerializeField]
    private Object? selectedAsset;
    #endregion Serialized

    #region View
    private ListView assetsView = null!;

    private Button refreshButton = null!;
    private Button exportSelectedButton = null!;

    private Label assetNameView = null!;

    private TextField typeNameView = null!;

    private VisualElement contentContainer = null!;

    private Image previewImage = null!;

    private Toggle isComponentToggle = null!;

    private Vector3Field positionOffsetView = null!;
    private Vector3Field rotationOffsetView = null!;
    private Vector3Field lightRotationOffsetView = null!;
    #endregion View

    #region Data
    private List<Object> assets = new();

    private int previewSize = 512;

    private Scene previewScene;

    private GameObject? cameraObj;

    private Camera? sceneCamera;

    private Object? previewAsset;

    private Light? previewLight;

    private GameObject? previewVolume;
    #endregion Data

    [MenuItem(EditorHelper.WINDOWS_TAB_NAME + "/" + EditorHelper.CCENVS_TAB + "/Thumbnail Creator")]
    public static void ShowExample()
    {
        ObjectThumbnailCreationWindow wnd = GetWindow<ObjectThumbnailCreationWindow>("Asset Thumbnail Creator");
        wnd.titleContent = new GUIContent("Asset Thumbnail Creator");
    }

    private void OnDisable()
    {
        EditorSceneManager.ClosePreviewScene(previewScene);
        SaveData();
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
        ResolveExportSelectedButton();
        ResolvePreviewImage();
        ResolvePositionOffsetView();
        ResolveRotationOffsetView();
        ResolveTypeNameView();
        ResolveIsComponentToggle();
        ResolveLightRotationOffsetView();
        RestoreSavedData();
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

    private void ResolveExportSelectedButton()
    {
        exportSelectedButton = rootVisualElement.Q<Button>("ExportSelected");
        exportSelectedButton.clicked += ExportTexturesAsync;
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

    private void ResolveLightRotationOffsetView()
    {
        lightRotationOffsetView = contentContainer.Q<Vector3Field>("LightRotationOffset");
        lightRotationOffsetView.RegisterValueChangedCallback(OnLightRotationOffsetChanged);
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

        ApplyPreviewPositionOffset();
        RenderPreviewScene();
    }

    private void OnRotationOffsetChanged(ChangeEvent<Vector3> ev)
    {
        if (previewAsset == null)
            return;

        ApplyPreviewRotationOffset();
        RenderPreviewScene();
    }

    private void OnLightRotationOffsetChanged(ChangeEvent<Vector3> ev)
    {
        ApplyLightRotationOffset();
        RenderPreviewScene();
    }

    private void OnSelectItem(IEnumerable<object> item)
    {
        if (!assetsView.selectedIndices.Maybe()
            .Where(indices => indices.IsNotEmpty())
            .Map(indices => indices.First())
            .TryGetValue(out var firstIdx)
            ||
            firstIdx >= assets.Count
            )
        {
            return;
        }

        selectedAsset = assets[firstIdx];
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

    private void RestoreSavedData()
    {
        EditorLibrary.Load<Snapshot>(SAVE_PATH)?.TryRestore(this);
    }

    private void SaveData()
    {
        var snapshot = new Snapshot().CaptureFrom(this);

        EditorLibrary.Save(SAVE_PATH, snapshot);
    }

    private void FindAssets()
    {
        assets.Clear();

        string typeFilter = typeNameView.value.IsNullOrWhiteSpace() || isComponentToggle.value ? "GameObject" : typeNameView.value;

        var dbAssets =
            from guid in AssetDatabase.FindAssets($"t:{typeFilter}")
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
                where cmp != null
                let cmpType = cmp.GetType()
                where cmpType.FullName.StartsWith(typeNameView.value) || cmpType.Name.StartsWith(typeNameView.value)
                select PrefabUtility.GetOutermostPrefabInstanceRoot(cmp) into prefab
                where prefab != null
                select prefab;
        }

        foreach (var asset in dbAssets)
            assets.Add(asset);

        assetsView.RefreshItems();
    }

    private void ApplyPreviewPositionOffset()
    {
        GetPreviewAssetTransform().Map(tr => tr.position = positionOffsetView.value);
    }

    private void ApplyPreviewRotationOffset()
    {
        GetPreviewAssetTransform().Map(tr => tr.rotation = Quaternion.Euler(rotationOffsetView.value));
    }

    private void ApplyLightRotationOffset()
    {
        previewLight.Maybe().IfSome(light => light.transform.rotation = Quaternion.Euler(lightRotationOffsetView.value));
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

        try
        {
            if (previewLight != null)
                DestroyImmediate(previewLight.gameObject);

            if (previewVolume != null)
                DestroyImmediate(previewVolume);
            previewLight = (Light)PrefabUtility.InstantiatePrefab(lightPrefab, previewScene);
            previewVolume = (GameObject)PrefabUtility.InstantiatePrefab(volumePrefab, previewScene);
        }
        catch (System.Exception ex)
        {
            this.PrintException(ex);
        }
    }

    private void ResolveCamera()
    {
        if (cameraObj != null || sceneCamera != null)
            return;

        sceneCamera =
            (from scene in previewScene.Maybe()
             where scene.IsValid()
             select scene.GetRootGameObjects() into objs
             from obj in objs
             select obj.Q().FromChildrens().IncludeInactive().Component<Camera>().Lax().GetValue() into camera
             where camera != null
             select camera
            )
            .FirstOrDefault();

        if (sceneCamera != null)
        {
            cameraObj = sceneCamera.gameObject;
            return;
        }

        cameraObj = new GameObject("Camera");
        cameraObj.transform.SetPositionAndRotation(new Vector3(0f, 0f, -10), Quaternion.identity);

        sceneCamera = cameraObj.AddComponent<Camera>();
        sceneCamera.aspect = 1f;
        sceneCamera.backgroundColor = Color.black;
        sceneCamera.clearFlags = CameraClearFlags.SolidColor;
        sceneCamera.allowHDR = false;

        sceneCamera.targetTexture = new RenderTexture(
            previewSize,
            previewSize,
            32,
            RenderTextureFormat.ARGB32
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
            previewAsset = PrefabUtility.InstantiatePrefab(selectedAsset, previewScene);
        }
        catch (System.Exception ex)
        {
            this.PrintException(ex);
        }

        ApplyPreviewPositionOffset();
        ApplyPreviewRotationOffset();
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
                TextureFormat.RGBA32,
                false
                );
        }

        RenderTexture.active = sceneCamera.targetTexture;

        previewTexture.ReadPixels(new Rect(0f, 0f, previewSize, previewSize), 0, 0);
        previewTexture.Apply();

        RenderTexture.active = null;    
    }

    private bool isExporting;

    private async void ExportTexturesAsync()
    {
        if (sceneCamera == null
            ||
            isExporting)
        {
            return;
        }

        isExporting = true;

        try
        {
            sceneCamera.depthTextureMode = DepthTextureMode.Depth;
            sceneCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);

            var saveDir = GetSaveDirectory();

            if (saveDir.IsNullOrWhiteSpace())
                return;

            if (!Directory.Exists(saveDir) || saveDir == Application.dataPath)
            {
                typeof(ObjectThumbnailCreationWindow).PrintError($"Cannot find part of path. Path: {saveDir}");
                return;
            }

            await CreatePNGsAsync(saveDir);

            sceneCamera.depthTextureMode = DepthTextureMode.None;
            sceneCamera.backgroundColor = Color.black;

            RenderPreviewScene();
        }
        catch (Exception ex)
        {
            typeof(ObjectThumbnailCreationWindow).PrintException(ex);
        }
        finally
        {
            isExporting = false;
        }
    }

    private string? GetSaveDirectory()
    {
        string defaultDir;

        if (assets.IsNotNullOrEmpty())
            defaultDir = AssetDatabase.GetAssetPath(assets.First());
        else
            defaultDir = Application.dataPath;

        var result = EditorUtility.OpenFolderPanel("Select a directory to save all icons", defaultDir, string.Empty);

        if (result.IsNullOrWhiteSpace())
            return defaultDir;

        return result;
    }

    private async ValueTask CreatePNGsAsync(string dirPath)
    {
        if (sceneCamera == null
            ||
            previewTexture == null)
        {
            return;
        }

        var selectedAssets = assetsView.selectedIndices.Select(idx => { try { return assets[idx]; } catch (Exception) { return null; } })
            .Where(asset => asset != null)
            .ToArray();

        var selectedAssetCached = selectedAsset;

        rootVisualElement.enabledSelf = false;

        try
        {
            foreach (var asset in selectedAssets)
            {
                selectedAsset = asset;
                ResolvePreviewAsset();
                RenderPreviewScene();
                await CreatePNGAsync(previewTexture, dirPath, asset!.name);
            }

            selectedAsset = selectedAssetCached;
        }
        finally
        {
            rootVisualElement.enabledSelf = true;
        }
    }

    private async ValueTask CreatePNGAsync(Texture2D texture, string dirPath, string fileName)
    {
        CC.Guard.IsNotNull(selectedAsset, nameof(selectedAsset));

        AssetNameHelper.RemoveTypePrefix(selectedAsset.GetType(), fileName);
        AssetNameHelper.AddTypePrefix(TypeofCache<Texture2D>.Type, fileName);

        var bytes = texture.EncodeToPNG();

        var filePath = Path.ChangeExtension(Path.Join(dirPath, fileName), ".png");

        await File.WriteAllBytesAsync(filePath, bytes);
    }

    [Serializable, SerializationDescriptor("ObjectThumbnailCreationWindow.Snapshot", "adc8d658-0cca-4bf5-8ca8-67ac798d2194")]
    public record Snapshot : CCEnvs.Snapshots.Snapshot<ObjectThumbnailCreationWindow>
    {
        [JsonProperty("previewPositionOffset")]
        public Vector3 PreviewPositionOffset { get; set; }

        [JsonProperty("previewRotationOffset")]
        public Vector3 PreviewRotationOffset { get; set; }

        [JsonProperty("lightRotationOffset")]
        public Vector3 LightRotationOffset { get; set; }

        protected override void OnRestore(ref ObjectThumbnailCreationWindow target)
        {
            target.positionOffsetView.value = PreviewPositionOffset;
            target.rotationOffsetView.value = PreviewRotationOffset;
            target.lightRotationOffsetView.value = LightRotationOffset;
        }

        protected override void OnCapture(ObjectThumbnailCreationWindow target)
        {
            base.OnCapture(target);
            PreviewPositionOffset = target.positionOffsetView.value;
            PreviewRotationOffset = target.rotationOffsetView.value;
            LightRotationOffset = target.lightRotationOffsetView.value;
        }

        protected override void OnReset()
        {
            base.OnReset();
            PreviewPositionOffset = default;
            PreviewPositionOffset = default;
            LightRotationOffset = default;
        }
    }
}
