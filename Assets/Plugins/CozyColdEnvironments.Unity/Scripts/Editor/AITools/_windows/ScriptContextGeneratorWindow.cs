#if UNITY_EDITOR
using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Serialization;
using CCEnvs.Snapshots;
using CCEnvs.Threading;
using CCEnvs.Threading.Tasks;
using CCEnvs.UnityX.UI.Elements;
using R3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Editr.AITools
{
    public sealed class ScriptContextGeneratorWindow : EditorWindow
    {
        [DataContract]
        [PolymorphSerializable, SerializationDescriptor("16dc10ed-571d-4db9-bf10-61f005877906")]
        public sealed record Snapshot : Snapshot<ScriptContextGeneratorWindow>
        {
            [DataMember(Name = "scriptInfos")]
            public ScriptContextScriptInfo[]? ScriptInfos { get; set; }

            [DataMember(Name = "directories")]
            public string[]? Directories { get; set; }

            [DataMember(Name = "excludeDirectories")]
            public string[]? ExcludeDirectories { get; set; }

            [DataMember(Name = "defaultMatchSettings")]
            public StringMatchSettings? DefaultMatchSettings { get; set; }

            [DataMember(Name = "aiOutput")]
            public string? AIOutput { get; set; }

            [DataMember(Name = "aiInput")]
            public string? AIInput { get; set; }

            public Snapshot(Snapshot<ScriptContextGeneratorWindow> original) : base(original) { }
            public Snapshot() { }
            public Snapshot(ScriptContextGeneratorWindow target) : base(target) { }

            protected override void OnRestore(ref ScriptContextGeneratorWindow target)
            {
                if (ScriptInfos is not null) target.scriptInfos.AddRange(ScriptInfos);
                if (Directories is not null) target.directories.AddRange(Directories);
                if (ExcludeDirectories is not null) target.excludeDirectories.AddRange(ExcludeDirectories);
                if (DefaultMatchSettings.HasValue) target.defaultMatchSettingsField.value = (int)DefaultMatchSettings.Value;
                if (AIOutput is not null) target.aiOutputField.value = AIOutput;
                if (AIInput is not null) target.aiInputField.value = AIInput;
            }

            protected override void OnCapture(ScriptContextGeneratorWindow target)
            {
                base.OnCapture(target);
                ScriptInfos = target.scriptInfos.ToArray();
                Directories = target.directories.ToArray();
                ExcludeDirectories = target.excludeDirectories.ToArray();
                DefaultMatchSettings = target.defaultMatchSettingsField is null ? null : (StringMatchSettings)Enum.ToObject(typeof(StringMatchSettings), target.defaultMatchSettingsField.value);
                AIOutput = target.aiOutputField?.value;
                AIInput = target.aiInputField?.value;
            }

            protected override void OnReset()
            {
                base.OnReset();
                ScriptInfos = null;
                Directories = null;
                ExcludeDirectories = null;
                DefaultMatchSettings = null;
                AIOutput = null;
                AIInput = null;
            }
        }

        private const string EDITOR_LIBRARY_FOLDER = "ScriptContextGeneratorWindow";
        private const string SAVE_PATH = EDITOR_LIBRARY_FOLDER + "/data.json";

        private const string SCRIPT_NAME_FIELD_NAME = "script-context-generator__script-name";
        private const string MATCH_SETTINGS_FIELD_NAME = "script-context-generator__script-name-match-settings";
        private const string ONLY_API_TOGGLE_NAME = "script-context-generator__only-api";
        private const string INCLUDE_DEPENDENCIES_TOGGLE_NAME = "script-context-generator__include-dependecies";
        private const string REMOVE_EMPTY_LINES_TOGGLE_NAME = "script-context-generator__remove-empty-lines";
        private const string DEPENDENCIES_ONLY_API_TOGGLE_NAME = "script-context-generator__dependecies-only-api";
        private const string DEPENDENCIES_REMOVE_EMPTY_LINES_TOGGLE_NAME = "script-context-generator__dependecies-remove-empty-lines";
        private const string DIRECTORY_FIELD_NAME = "script-context-generator__directory-text-field";
        private const string EXCLUDE_DIRECTORY_FIELD_NAME = DIRECTORY_FIELD_NAME;
        private const string DEPENDENCIES_MAX_DEPTH_FIELD_NAME = "script-context-generator__dependencies-max-depth-int-field";

        private readonly List<ScriptContextScriptInfo> scriptInfos = new();
        private readonly List<string> directories = new();
        private readonly List<string> excludeDirectories = new();
        private readonly CCCompositeDisposable bindings = new();
        private readonly CancellationTokenSource destroyCancellationTokenSource = new();
        private readonly Dictionary<VisualElement, CCCompositeDisposable> itemBindings = new();

        [SerializeField] private VisualTreeAsset view = null!;

        private ListView scriptInfosListView = null!;
        private ListView directoriesListView = null!;
        private ListView excludeDirectoriesListView = null!;
        private Button generateButton = null!;
        private Button cancelGenerateButton = null!;
        private Button clearAIOutputButton = null!;
        private Button applyAIOutputButton = null!;
        private Button cancelApplyingAIOutputButton = null!;
        private MaskField defaultMatchSettingsField = null!;
        private TextField aiOutputField = null!;
        private TextField aiInputField = null!;

        private bool isGenerating;
        private bool isApllyingAIOutput;
        private CancellationTokenSource? disableCancellationTokenSource = new();
        private CancellationTokenSource? cancelGenerateCancellationTokenSource = new();
        private CancellationTokenSource? cancelApplyingAIOutputCancellationTokenSource = new();

        private CancellationToken destroyCancellationToken => destroyCancellationTokenSource.Token;
        private CancellationToken disableCancellationToken => disableCancellationTokenSource?.Token ?? default;
        private CancellationToken cancelGenerateCancellationToken => cancelGenerateCancellationTokenSource?.Token ?? default;
        private CancellationToken cancelApplyingAIOutputCancellationToken => cancelApplyingAIOutputCancellationTokenSource?.Token ?? default;

        private string PersistentQueryPath => Path.Combine(EditorLibrary.DataPath, "PersistentQuery.txt");

        public void CreateGUI()
        {
            if (view == null)
            {
                this.PrintError(DebugMessageBuilder.CreatePooled().AddMessage("View is null").ToStringAndDispose());
                return;
            }

            var root = rootVisualElement;
            InitRootElement(root);
            InitStringMatchSettingsField(defaultMatchSettingsField);
            defaultMatchSettingsField.value = (int)StringMatchSettings.Ordinal;

            SetupDirectoriesListView();
            SetupExcludeDirectoriesListView();
            SetupScriptInfosListView();
            InitPersistentQueryFile();

            BindGenerateButton();
            BindCancelGenerateButton();
            BindClearAIOutputButton();
            BindApplyAIOutputButton();
            BindCancelApplyingAIOutputButton();
            RestoreSavedData();
        }

        private void OnEnable()
        {
            disableCancellationTokenSource?.CancelAndDispose();
            disableCancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDisable() => SaveData();

        private void OnDestroy()
        {
            bindings.Dispose();
            itemBindings.SelectValue().DisposeEach();
            itemBindings.Clear();
            destroyCancellationTokenSource.CancelAndDispose();
            disableCancellationTokenSource?.CancelAndDispose();
            cancelGenerateCancellationTokenSource?.CancelAndDispose();
            cancelApplyingAIOutputCancellationTokenSource?.CancelAndDispose();
        }

        [MenuItem(EditorHelper.WINDOWS_TAB_NAME + "/" + EditorHelper.CCENVS_TAB + "/" + EditorHelper.AI_TAB + "/" + "Script Context Generator")]
        public static void ShowExample() => GetWindow<ScriptContextGeneratorWindow>("Script Context Generator");

        private void SaveData()
        {
            var snapshot = new Snapshot(this);
            snapshot.CaptureFrom(this);
            EditorLibrary.Save(SAVE_PATH, snapshot);
        }

        private void RestoreSavedData()
        {
            try
            {
                if (EditorLibrary.Load<Snapshot>(SAVE_PATH).IsNotNull(out var snapshot))
                    snapshot.TryRestore(this);
            }
            catch (Exception ex) { this.PrintException(ex); }
        }

        private void InitRootElement(VisualElement root)
        {
            root.Add(view.Instantiate());
            scriptInfosListView = root.QStrict<ListView>("script-context-generator__script-names-list-view");
            directoriesListView = root.QStrict<ListView>("script-context-generator__directories-list-view");
            excludeDirectoriesListView = root.QStrict<ListView>("script-context-generator__exclude-directories-list-view");
            generateButton = root.QStrict<Button>("script-context-generator__generate-button");
            defaultMatchSettingsField = root.QStrict<MaskField>("script-context-generator__script-name-match-settings");
            cancelGenerateButton = root.QStrict<Button>("script-context-generator__cancel-generate-button");
            aiOutputField = root.QStrict<TextField>("script-context-generator__ai-output-text-field");
            clearAIOutputButton = root.QStrict<Button>("script-context-generator__clear-ai-output-button");
            aiInputField = root.QStrict<TextField>("script-context-generator__ai-input-text-field");
            applyAIOutputButton = root.QStrict<Button>("script-context-generator__apply-ai-output-button");
            cancelApplyingAIOutputButton = root.QStrict<Button>("script-context-generator__cancel-applying-ai-output-button");
        }

        private void InitStringMatchSettingsField(MaskField maskField)
        {
            var flags = Enum.GetValues(typeof(StringMatchSettings)).Cast<StringMatchSettings>().ToArray();
            maskField.choices = flags.Select(x => x.ToString()).ToList();
            maskField.choicesMasks = flags.Select(x => x.ToIntUnsafe()).ToList();
        }

        private void InitPersistentQueryFile()
        {
            try
            {
                if (!File.Exists(PersistentQueryPath))
                    File.WriteAllText(PersistentQueryPath, $"Каждый файл помещается в подпапку в зависимости от его типа.{Environment.NewLine}Class/Record => _classes.{Environment.NewLine}static => _helpers. struct => _structs.{Environment.NewLine}interfaces => _interfaces.{Environment.NewLine}ISystem (DOTS) => _ecs/_systems.{Environment.NewLine}Component с данными (DOTS) => _ecs/_components.{Environment.NewLine}Component без данных (DOTS) => _ecs/_tags. {Environment.NewLine}Элемент буффера (DOTS) => _ecs/_buffers. {Environment.NewLine}Авторинг (DOTS) => _ecs/_autrhorings");
            }
            catch (Exception ex)
            {
                this.PrintWarning(ex);
            }
        }

        private void SetupScriptInfosListView()
        {
            VisualElement onMakeItem()
            {
                var root = scriptInfosListView.itemTemplate.Instantiate();
                var field = root.QStrict<TextField>(SCRIPT_NAME_FIELD_NAME);
                var matchSettingsField = root.QStrict<MaskField>(MATCH_SETTINGS_FIELD_NAME);
                var onlyApiToggle = root.QStrict<Toggle>(ONLY_API_TOGGLE_NAME);
                var includeDependenciesToggle = root.QStrict<Toggle>(INCLUDE_DEPENDENCIES_TOGGLE_NAME);
                var removeEmptyLinesToggle = root.QStrict<Toggle>(REMOVE_EMPTY_LINES_TOGGLE_NAME);
                var dependenciesOnlyApiToggle = root.QStrict<Toggle>(DEPENDENCIES_ONLY_API_TOGGLE_NAME);
                var dependenciesRemoveEmptyLinesToggle = root.QStrict<Toggle>(DEPENDENCIES_REMOVE_EMPTY_LINES_TOGGLE_NAME);
                var dependenciesMaxDepthField = root.QStrict<IntegerField>(DEPENDENCIES_MAX_DEPTH_FIELD_NAME);

                InitStringMatchSettingsField(matchSettingsField);
                matchSettingsField.value = defaultMatchSettingsField.value;

                void onFieldValueChanged(ChangeEvent<string> ev) => scriptInfos[field.userData.CastTo<int>()].Name = ev.newValue;
                void onMatchSettingsFieldValueChanged(ChangeEvent<int> ev) => scriptInfos[field.userData.CastTo<int>()].MatchSettings = (StringMatchSettings)Enum.ToObject(typeof(StringMatchSettings), ev.newValue);
                void onOnlyAPIToggleValueChanged(ChangeEvent<bool> ev) => scriptInfos[field.userData.CastTo<int>()].OnlyAPI = ev.newValue;
                void onIncludeDependenciesToggleValueChanged(ChangeEvent<bool> ev) => scriptInfos[field.userData.CastTo<int>()].IncludeDependecies = ev.newValue;
                void onRemoveEmptyLinesValueChanged(ChangeEvent<bool> ev) => scriptInfos[field.userData.CastTo<int>()].RemoveEmptyLines = ev.newValue;
                void onDependenciesOnlyApiValueChanged(ChangeEvent<bool> ev) => scriptInfos[field.userData.CastTo<int>()].DependeciesOnlyAPI = ev.newValue;
                void onDependenciesRemoveEmptyLinesValueChanged(ChangeEvent<bool> ev) => scriptInfos[field.userData.CastTo<int>()].DependeciesRemoveEmptyLines = ev.newValue;
                void onDependenciesMaxDepthValueChanged(ChangeEvent<int> ev) => scriptInfos[field.userData.CastTo<int>()].DependenciesMaxDepth = ev.newValue;

                var itemBindings = this.itemBindings.GetOrCreateNew(root);

                if (field != null)
                {
                    field.RegisterValueChangedCallback(onFieldValueChanged);
                    CCDisposable.Create(() => field.UnregisterValueChangedCallback(onFieldValueChanged)).AddTo(itemBindings);
                }

                if (matchSettingsField != null)
                {
                    matchSettingsField.RegisterValueChangedCallback(onMatchSettingsFieldValueChanged);
                    CCDisposable.Create(() => matchSettingsField.UnregisterValueChangedCallback(onMatchSettingsFieldValueChanged)).AddTo(itemBindings);
                }

                if (onlyApiToggle != null)
                {
                    onlyApiToggle.RegisterValueChangedCallback(onOnlyAPIToggleValueChanged);
                    CCDisposable.Create(() => onlyApiToggle.UnregisterValueChangedCallback(onOnlyAPIToggleValueChanged)).AddTo(itemBindings);
                }

                if (includeDependenciesToggle != null)
                {
                    includeDependenciesToggle.RegisterValueChangedCallback(onIncludeDependenciesToggleValueChanged);
                    CCDisposable.Create(() => includeDependenciesToggle.UnregisterValueChangedCallback(onIncludeDependenciesToggleValueChanged)).AddTo(itemBindings);
                }

                if (removeEmptyLinesToggle != null)
                {
                    removeEmptyLinesToggle.RegisterValueChangedCallback(onRemoveEmptyLinesValueChanged);
                    CCDisposable.Create(() => removeEmptyLinesToggle.UnregisterValueChangedCallback(onRemoveEmptyLinesValueChanged)).AddTo(itemBindings);
                }

                if (dependenciesOnlyApiToggle != null)
                {
                    dependenciesOnlyApiToggle.RegisterValueChangedCallback(onDependenciesOnlyApiValueChanged);
                    CCDisposable.Create(() => dependenciesOnlyApiToggle.UnregisterValueChangedCallback(onDependenciesOnlyApiValueChanged)).AddTo(itemBindings);
                }

                if (dependenciesRemoveEmptyLinesToggle != null)
                {
                    dependenciesRemoveEmptyLinesToggle.RegisterValueChangedCallback(onDependenciesRemoveEmptyLinesValueChanged);
                    CCDisposable.Create(() => dependenciesRemoveEmptyLinesToggle.UnregisterValueChangedCallback(onDependenciesRemoveEmptyLinesValueChanged)).AddTo(itemBindings);
                }

                if (dependenciesMaxDepthField != null)
                {
                    dependenciesMaxDepthField.RegisterValueChangedCallback(onDependenciesMaxDepthValueChanged);
                    CCDisposable.Create(() => dependenciesMaxDepthField.UnregisterValueChangedCallback(onDependenciesMaxDepthValueChanged)).AddTo(itemBindings);
                }

                return root;
            }

            void onDestroyItem(VisualElement item)
            {
                if (itemBindings.TryGetValue(item, out var b)) b.Dispose();
            }

            void onAdd(BaseListView list)
            {
                scriptInfos.Add(new ScriptContextScriptInfo()
                {
                    Name = string.Empty,
                    MatchSettings = (StringMatchSettings)Enum.ToObject(typeof(StringMatchSettings), defaultMatchSettingsField.value),
                    OnlyAPI = false,
                    IncludeDependecies = false,
                    RemoveEmptyLines = false,
                    DependeciesOnlyAPI = true,
                    DependeciesRemoveEmptyLines = false,
                    DependenciesMaxDepth = 6
                });
            }

            void onRemove(BaseListView _)
            {
                bool hasSelected = scriptInfosListView.selectedIndices.IsNotEmpty();
                if (hasSelected)
                {
                    bool removed = false;
                    foreach (var index in scriptInfosListView.selectedIndices.OrderByDescending(x => x))
                    {
                        if (index >= scriptInfos.Count)
                            continue;

                        scriptInfos.RemoveAt(index);
                        removed = true;
                    }

                    if (!removed)
                        scriptInfos.RemoveAt(scriptInfos.Count - 1);
                }
                else
                {
                    scriptInfos.RemoveAt(scriptInfos.Count - 1);
                }
                scriptInfosListView.RefreshItems();
            }

            void onBindItem(VisualElement item, int index)
            {
                var field = item.QStrict<TextField>(SCRIPT_NAME_FIELD_NAME);
                var matchSettingsField = item.QStrict<MaskField>(MATCH_SETTINGS_FIELD_NAME);
                var onlyApiToggle = item.QStrict<Toggle>(ONLY_API_TOGGLE_NAME);
                var includeDependenciesToggle = item.QStrict<Toggle>(INCLUDE_DEPENDENCIES_TOGGLE_NAME);
                var removeEmptyLinesToggle = item.QStrict<Toggle>(REMOVE_EMPTY_LINES_TOGGLE_NAME);
                var dependenciesOnlyApiToggle = item.QStrict<Toggle>(DEPENDENCIES_ONLY_API_TOGGLE_NAME);
                var dependenciesRemoveEmptyLinesToggle = item.QStrict<Toggle>(DEPENDENCIES_REMOVE_EMPTY_LINES_TOGGLE_NAME);
                var dependenciesMaxDepthField = item.QStrict<IntegerField>(DEPENDENCIES_MAX_DEPTH_FIELD_NAME);

                if (index >= scriptInfos.Count)
                {
                    throw new InvalidOperationException(DebugMessageBuilder.CreatePooled().AddMessage("Not found script info at index").AddProperty("Index", index).ToStringAndDispose());
                }

                var info = scriptInfos[index];
                field.userData = index;
                field.value = info.Name;
                matchSettingsField.value = (int)info.MatchSettings;
                onlyApiToggle.value = info.OnlyAPI;
                includeDependenciesToggle.value = info.IncludeDependecies;
                removeEmptyLinesToggle.value = info.RemoveEmptyLines;
                dependenciesOnlyApiToggle.value = info.DependeciesOnlyAPI;
                dependenciesRemoveEmptyLinesToggle.value = info.DependeciesRemoveEmptyLines;

                if (dependenciesMaxDepthField != null)
                    dependenciesMaxDepthField.value = info.DependenciesMaxDepth;
            }

            if (scriptInfosListView != null)
            {
                scriptInfosListView.makeItem += onMakeItem;
                scriptInfosListView.destroyItem += onDestroyItem;
                scriptInfosListView.onAdd += onAdd;
                scriptInfosListView.onRemove += onRemove;
                scriptInfosListView.bindItem += onBindItem;
                scriptInfosListView.itemsSource = scriptInfos;
            }
        }

        private void SetupDirectoriesListView()
        {
            VisualElement onMakeItem()
            {
                var root = directoriesListView.itemTemplate.Instantiate();
                var field = root.QStrict<TextField>(DIRECTORY_FIELD_NAME);

                void onFieldValueChanged(ChangeEvent<string> ev)
                {
                    directories[field.userData.CastTo<int>()] = ev.newValue;
                }

                var itemBindings = this.itemBindings.GetOrCreateNew(root);

                if (field != null)
                {
                    field.RegisterValueChangedCallback(onFieldValueChanged);
                    CCDisposable.Create(() => field.UnregisterValueChangedCallback(onFieldValueChanged)).AddTo(itemBindings);
                }

                return root;
            }

            void onDestroyItem(VisualElement item)
            {
                if (itemBindings.TryGetValue(item, out var bindings))
                    bindings.Dispose();
            }

            void onAdd(BaseListView list)
            {
                directories.Add(string.Empty);
            }

            void onRemove(BaseListView _)
            {
                bool hasSelected = directoriesListView.selectedIndices.IsNotEmpty();

                if (hasSelected)
                {
                    foreach (var index in directoriesListView.selectedIndices.OrderByDescending(x => x))
                        directories.RemoveAt(index);
                }
                else
                {
                    directories.RemoveAt(directories.Count - 1);
                }

                directoriesListView.RefreshItems();
            }

            void onBindItem(VisualElement item, int index)
            {
                var field = item.QStrict<TextField>(DIRECTORY_FIELD_NAME);

                if (index >= directories.Count)
                {
                    throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                        .AddMessage("Not found directory at index")
                        .AddProperty("Index", index)
                        .ToStringAndDispose()
                        );
                }

                field.userData = index;
                field.value = directories[index];
            }

            void onUnbindItem(VisualElement item, int index) { }

            if (directoriesListView != null)
            {
                directoriesListView.makeItem += onMakeItem;
                directoriesListView.destroyItem += onDestroyItem;
                directoriesListView.onAdd += onAdd;
                directoriesListView.onRemove += onRemove;
                directoriesListView.bindItem += onBindItem;
                directoriesListView.unbindItem += onUnbindItem;
                directoriesListView.itemsSource = directories;
            }
        }

        private void SetupExcludeDirectoriesListView()
        {
            VisualElement onMakeItem()
            {
                var root = excludeDirectoriesListView.itemTemplate.Instantiate();
                var field = root.QStrict<TextField>(EXCLUDE_DIRECTORY_FIELD_NAME);

                void onFieldValueChanged(ChangeEvent<string> ev)
                {
                    excludeDirectories[field.userData.CastTo<int>()] = ev.newValue;
                }

                var itemBindings = this.itemBindings.GetOrCreateNew(root);

                if (field != null)
                {
                    field.RegisterValueChangedCallback(onFieldValueChanged);
                    CCDisposable.Create(() => field.UnregisterValueChangedCallback(onFieldValueChanged)).AddTo(itemBindings);
                }

                return root;
            }

            void onDestroyItem(VisualElement item)
            {
                if (itemBindings.TryGetValue(item, out var bindings))
                    bindings.Dispose();
            }

            void onAdd(BaseListView list)
            {
                excludeDirectories.Add(string.Empty);
            }

            void onRemove(BaseListView _)
            {
                bool hasSelected = excludeDirectoriesListView.selectedIndices.IsNotEmpty();

                if (hasSelected)
                {
                    foreach (var index in excludeDirectoriesListView.selectedIndices.OrderByDescending(x => x))
                        excludeDirectories.RemoveAt(index);
                }
                else
                {
                    excludeDirectories.RemoveAt(excludeDirectories.Count - 1);
                }

                excludeDirectoriesListView.RefreshItems();
            }

            void onBindItem(VisualElement item, int index)
            {
                var field = item.QStrict<TextField>(EXCLUDE_DIRECTORY_FIELD_NAME);

                if (index >= excludeDirectories.Count)
                {
                    throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                        .AddMessage("Not found exclude directory at index")
                        .AddProperty("Index", index)
                        .ToStringAndDispose()
                        );
                }

                field.userData = index;
                field.value = excludeDirectories[index];
            }

            void onUnbindItem(VisualElement item, int index) { }

            if (excludeDirectoriesListView != null)
            {
                excludeDirectoriesListView.makeItem += onMakeItem;
                excludeDirectoriesListView.destroyItem += onDestroyItem;
                excludeDirectoriesListView.onAdd += onAdd;
                excludeDirectoriesListView.onRemove += onRemove;
                excludeDirectoriesListView.bindItem += onBindItem;
                excludeDirectoriesListView.unbindItem += onUnbindItem;
                excludeDirectoriesListView.itemsSource = excludeDirectories;
            }
        }

        private void BindGenerateButton()
        {
            if (generateButton != null)
            {
                generateButton.clicked += GenerateAndSaveToFile;
                CCDisposable.Create(() => generateButton.clicked -= GenerateAndSaveToFile).AddTo(bindings);
            }
        }

        private void CancelGenerate()
        {
            if (!isGenerating) return;
            cancelGenerateCancellationTokenSource?.CancelAndDispose();
            cancelGenerateCancellationTokenSource = new CancellationTokenSource();
            isGenerating = false;
            generateButton.enabledSelf = true;
            cancelGenerateButton.enabledSelf = false;
        }

        private void BindCancelGenerateButton()
        {
            if (cancelGenerateButton != null)
            {
                cancelGenerateButton.clicked += CancelGenerate;
                CCDisposable.Create(() => cancelGenerateButton.clicked -= CancelGenerate).AddTo(bindings);
            }
        }

        private async void GenerateAndSaveToFile()
        {
            if (isGenerating) return;

            isGenerating = true;
            generateButton.enabledSelf = false;
            cancelGenerateButton.enabledSelf = true;

            try
            {
                using var _ = disableCancellationToken.TryLinkTokens(cancelGenerateCancellationToken, out var cancellationToken);

                var context = await ScriptContextGenerator.GenerateAsync(
                    scriptInfos,
                    directories.IsNotNullOrEmpty() ? directories : null,
                    excludeDirectories.IsNotNullOrEmpty() ? excludeDirectories : null,
                    cancellationToken: cancellationToken);

                var contextDirPath = Path.Combine(EditorLibrary.DataPath, EDITOR_LIBRARY_FOLDER);

                if (!Directory.Exists(contextDirPath))
                    Directory.CreateDirectory(contextDirPath);

                var contextFilePath = Path.Combine(contextDirPath, "Context.txt");
                var additionalQuery = aiInputField.value;

                try
                {
                    if (File.Exists(PersistentQueryPath))
                        additionalQuery += $"{Environment.NewLine}{File.ReadAllText(PersistentQueryPath)}";
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }

                await File.WriteAllTextAsync(contextFilePath, context.ToString(additionalQuery), cancellationToken);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
            finally
            {
                isGenerating = false;
                generateButton.enabledSelf = true;
                cancelGenerateButton.enabledSelf = false;
            }
        }

        private void ClearAIOUtput()
        {
            aiOutputField.value = string.Empty;
        }

        private void BindClearAIOutputButton()
        {
            if (clearAIOutputButton != null)
            {
                clearAIOutputButton.clicked += ClearAIOUtput;
                CCDisposable.Create(() => clearAIOutputButton.clicked -= ClearAIOUtput).AddTo(bindings);
            }
        }

        private void ApplyAIOutput()
        {
            if (isApllyingAIOutput)
                return;

            isApllyingAIOutput = true;
            var cancellationTokenSource = destroyCancellationToken.TryLinkTokens(cancelApplyingAIOutputCancellationToken, out var cancellationToken);
            cancelApplyingAIOutputButton.enabledSelf = true;

            ScriptRewriter.ApplyAsync(aiOutputField.value, cancellationToken)
                .Finally(cancellationTokenSource, (cancellationTokenSource) =>
                {
                    cancellationTokenSource?.Dispose();
                    isApllyingAIOutput = false;
                    cancelApplyingAIOutputButton.enabledSelf = false;
                })
                .Forget(ex => this.PrintException(ex));
        }

        private void BindApplyAIOutputButton()
        {
            if (applyAIOutputButton != null)
            {
                applyAIOutputButton.clicked += ApplyAIOutput;
                CCDisposable.Create(() => applyAIOutputButton.clicked -= ApplyAIOutput).AddTo(bindings);
            }
        }

        private void BindCancelApplyingAIOutputButton()
        {
            void onClicked()
            {
                if (!isApllyingAIOutput)
                    return;

                cancelApplyingAIOutputCancellationTokenSource?.CancelAndDispose();
                isApllyingAIOutput = false;
                cancelApplyingAIOutputButton.enabledSelf = false;
            }

            if (cancelApplyingAIOutputButton != null)
            {
                cancelApplyingAIOutputButton.clicked += onClicked;
                CCDisposable.Create(() => cancelApplyingAIOutputButton.clicked -= onClicked).AddTo(bindings);
            }
        }
    }
}
#endif