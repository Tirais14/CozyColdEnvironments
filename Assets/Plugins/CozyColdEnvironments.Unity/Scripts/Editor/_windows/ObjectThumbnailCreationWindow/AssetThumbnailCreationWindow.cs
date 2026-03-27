using CCEnvs;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.Snapshots;
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
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

#nullable enable
[SnapshotConvertible]
public class AssetThumbnailCreationWindow : EditorWindow
{
    public const string SAVE_PATH = "ObjectThumbnailCreationWindow" + "/data.json";

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
    private Button saveButton = null!;

    private Label assetNameView = null!;

    private VisualElement contentContainer = null!;

    private Image previewImage = null!;

    private Toggle isComponentToggle = null!;
    private Toggle exportInSourceDirectoryToggle = null!;

    private TextField typeNameView = null!;

    private IntegerField previewSizeView = null!;

    private Vector3Field positionOffsetView = null!;
    private Vector3Field rotationOffsetView = null!;
    private Vector3Field lightRotationOffsetView = null!;
    #endregion View

    #region Data
    private List<Object> assets = new();

    private Scene previewScene;

    private GameObject? cameraObj;

    private Camera? sceneCamera;

    private Object? previewAsset;

    private Light? previewLight;

    private GameObject? previewVolume;

    [SnapshotProperty("33548307-d33e-4082-ab00-62a6fc213edb")]
    private Dictionary<string, AnonymousSnapshot> perAssetSettings = new();
    #endregion Data

    [SnapshotProperty("1817f9df-957a-48ff-9bde-daa40b3dc3df")]
    public int PreviewSize {
        get => previewSizeView.value;
        set => previewSizeView.value = Math.Max(value, 8);
    }

    [SnapshotProperty("566bc544-a15e-46c2-be16-4647291b9c0c")]
    public string? TypeNameFilter {
        get => typeNameView.value;
        set => typeNameView.value = value;
    }

    [SnapshotProperty("3269315d-0c3c-4ac0-8d97-6e96fe0d6ab5")]
    public bool IsComponent {
        get => isComponentToggle.value;
        set => isComponentToggle.value = value;
    }

    [SnapshotProperty("50398fa4-053e-475b-b2f0-551aea05aba7")]
    public bool ExportInSourceDirectory {
        get => exportInSourceDirectoryToggle.value;
        set => exportInSourceDirectoryToggle.value = value;
    }

    [SnapshotProperty("35404730-841f-4442-a67e-48a82e110301")]
    public Vector3 PositionOffset {
        get => positionOffsetView.value;
        set => positionOffsetView.value = value;
    }

    [SnapshotProperty("6d549084-37bf-483c-8f32-d1744b872613")]
    public Vector3 RotationOffset {
        get => rotationOffsetView.value;
        set => rotationOffsetView.value = value;
    }

    [SnapshotProperty("faf7df89-2422-40bf-b68d-0a1047bd1f60")]
    public Vector3 LightRotationOffset {
        get => lightRotationOffsetView.value;
        set => lightRotationOffsetView.value = value;
    }

    [MenuItem(EditorHelper.WINDOWS_TAB_NAME + "/" + EditorHelper.CCENVS_TAB + "/Thumbnail Creator")]
    public static void ShowExample()
    {
        AssetThumbnailCreationWindow wnd = GetWindow<AssetThumbnailCreationWindow>("Asset Thumbnail Creator");
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
        ResolveSaveButton();
        ResolvePreviewImage();
        ResolvePositionOffsetView();
        ResolveRotationOffsetView();
        ResolveTypeNameView();
        ResolveIsComponentToggle();
        ResolveExportInSourceDirectoryToggle();
        ResolveLightRotationOffsetView();
        ResolvePreviewSizeView();
        ResolvePreviewSizeView();
        RestoreSavedData();
        FindAssets();
        RenderPreviewScene();
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

    private void ResolvePreviewSizeView()
    {
        previewSizeView = contentContainer.Q<IntegerField>("PreviewSize");
        previewSizeView.value = 512;
        previewSizeView.RegisterValueChangedCallback(OnPreviewSizeChanged);
    }

    private void ResolveAssetNameView()
    {
        assetNameView = contentContainer.Q<Label>("AssetName");
        assetNameView.dataSource = this;
    }

    private void ResolveExportInSourceDirectoryToggle()
    {
        exportInSourceDirectoryToggle = contentContainer.Q<Toggle>("ExportInSourceDirectory");
    }

    private void ResolveSaveButton()
    {
        saveButton = rootVisualElement.Q<Button>("Save");
        saveButton.clicked += SaveData;
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

        SetSelectedAsset(assets[firstIdx]);
        ResolvePreviewScene();
        ResolveCamera();
        ResolvePreviewAsset();
        RenderPreviewScene();
    }

    private void OnIsComponentToggleChanged(ChangeEvent<bool> ev)
    {
        FindAssets();
    }

    private void OnPreviewSizeChanged(ChangeEvent<int> ev)
    {
        ResolveRenderTexture();
        RenderPreviewScene();
    }
    #endregion Callbacks

    private void SetSelectedAsset(Object? asset)
    {
        if (selectedAsset != null)
            SaveWindowStateByAsset(selectedAsset);

        if (asset != null && selectedAsset != asset)
            RestoreWindowStateByAsset(asset);

        selectedAsset = asset;
    }

    private void RestoreWindowStateByAsset(Object asset)
    {
        var assetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));

        if (!perAssetSettings.TryGetValue(assetGuid, out var winSnapshot))
            return;

        winSnapshot.TryRestore(this);
    }

    private void SaveWindowStateByAsset(Object asset)
    {
        var assetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));

        var winSnapshot = perAssetSettings.GetOrCreate(assetGuid, () => Snapshot.Create(this));

        winSnapshot.CaptureFrom(this);
        winSnapshot.RemoveMemberByName(nameof(TypeNameFilter), out _);
        winSnapshot.RemoveMemberByName(nameof(IsComponent), out _);
        winSnapshot.RemoveMemberByName(nameof(ExportInSourceDirectory), out _);
        winSnapshot.RemoveMemberByName(nameof(perAssetSettings), out _);
    }

    private void RestoreSavedData()
    {
        EditorLibrary.Load<ISnapshot>(SAVE_PATH)?.TryRestore(this);
    }

    private void SaveData()
    {
        var snapshot = Snapshot.Create(this);

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

        foreach (var asset in dbAssets.Distinct())
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

    private void ResolveRenderTexture()
    {
        if (sceneCamera == null
            ||
            (sceneCamera.targetTexture != null
            &&
            sceneCamera.targetTexture.width == PreviewSize
            &&
            sceneCamera.targetTexture.height == PreviewSize
            ))
        {
            return;
        }

        sceneCamera.targetTexture = new RenderTexture(
            PreviewSize,
            PreviewSize,
            32,
            RenderTextureFormat.ARGB32
            );
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
        sceneCamera.allowHDR = true;
        sceneCamera.allowMSAA = true;

        var cameraData = cameraObj.AddComponent<UniversalAdditionalCameraData>();

        cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
        cameraData.antialiasingQuality = AntialiasingQuality.High;
        cameraData.renderPostProcessing = true;

        ResolveRenderTexture();

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

        ResolvePreviewTexture();

        RenderTexture.active = sceneCamera.targetTexture;

        previewTexture!.ReadPixels(new Rect(0f, 0f, PreviewSize, PreviewSize), 0, 0);
        previewTexture!.Apply();

        RenderTexture.active = null;    
    }

    private void ResolvePreviewTexture()
    {
        if (previewTexture != null
            &&
            previewTexture.width == PreviewSize
            &&
            previewTexture.height == PreviewSize
            )
        {
            return;
        }

        previewTexture = new Texture2D(
            PreviewSize,
            PreviewSize,
            TextureFormat.RGBA32,
            false
            );
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

            if (!exportInSourceDirectoryToggle.value)
            {
                var saveDir = GetSaveDirectory();

                if (saveDir.IsNullOrWhiteSpace())
                    return;

                if (!Directory.Exists(saveDir) || saveDir == Application.dataPath)
                {
                    typeof(AssetThumbnailCreationWindow).PrintError($"Cannot find part of path. Path: {saveDir}");
                    return;
                }

                await CreatePNGsAsync(saveDir);
            }
            else
                await CreatePNGsAsync(null);

            sceneCamera.depthTextureMode = DepthTextureMode.None;
            sceneCamera.backgroundColor = Color.black;

            RenderPreviewScene();
        }
        catch (Exception ex)
        {
            typeof(AssetThumbnailCreationWindow).PrintException(ex);
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

    private async ValueTask CreatePNGsAsync(string? dirPath)
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
                SetSelectedAsset(asset);
                ResolvePreviewAsset();
                RenderPreviewScene();

                var path = dirPath ?? Path.GetDirectoryName(AssetDatabase.GetAssetPath(asset));

                await CreatePNGAsync(previewTexture, path, asset!.name);
            }

            SetSelectedAsset(selectedAssetCached);
        }
        finally
        {
            rootVisualElement.enabledSelf = true;
        }
    }

    private async ValueTask CreatePNGAsync(Texture2D texture, string dirPath, string fileName)
    {
        CC.Guard.IsNotNull(selectedAsset, nameof(selectedAsset));

        fileName = AssetNameHelper.RemoveTypePrefix(selectedAsset.GetType(), fileName);
        fileName = AssetNameHelper.AddTypePrefix(TypeofCache<Texture2D>.Type, fileName);

        var bytes = texture.EncodeToPNG();

        var filePath = Path.ChangeExtension(Path.Join(dirPath, fileName), ".png");

        AssetDatabase.LoadAssetAtPath<Texture2D>(filePath).Maybe().IfSome(tx =>
        {
            tx.alphaIsTransparency = true;

            AssetDatabase.SaveAssetIfDirty(tx);
        });

        await File.WriteAllBytesAsync(filePath, bytes);
    }
}
