#if UNITY_EDITOR
using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Serialization;
using CCEnvs.Snapshots;
using CCEnvs.Threading;
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
        [PolymorphSerializable, SerializationDescriptor("a49888a0-8b14-49ef-a6ca-4c21cb25487d")]
        public sealed record Snapshot : Snapshot<ScriptContextGeneratorWindow>
        {
            [DataMember(Name = "scriptInfos")]
            public ScriptContextScriptInfo[]? ScriptInfos { get; set; }

            [DataMember(Name = "directories")]
            public string[]? Directories { get; set; }

            [DataMember(Name = "defaultMatchSettings")]
            public StringMatchSettings? DefaultMatchSettings { get; set; }

            [DataMember(Name = "dependencyMaxDepth")]
            public int DependencyMaxDepth { get; set; } = 6;

            public Snapshot(Snapshot<ScriptContextGeneratorWindow> original) : base(original) { }
            public Snapshot() { }
            public Snapshot(ScriptContextGeneratorWindow target) : base(target) { }

            protected override void OnRestore(ref ScriptContextGeneratorWindow target)
            {
                if (ScriptInfos is not null) target.scriptInfos.AddRange(ScriptInfos);
                if (Directories is not null) target.directories.AddRange(Directories);
                if (DefaultMatchSettings.HasValue) target.defaultMatchSettingsField.value = (int)DefaultMatchSettings.Value;
                target.dependencyMaxDepthField.value = DependencyMaxDepth;
            }

            protected override void OnCapture(ScriptContextGeneratorWindow target)
            {
                base.OnCapture(target);
                ScriptInfos = target.scriptInfos.ToArray();
                Directories = target.directories.ToArray();
                DefaultMatchSettings = (StringMatchSettings)Enum.ToObject(typeof(StringMatchSettings), target.defaultMatchSettingsField.value);
                DependencyMaxDepth = target.dependencyMaxDepthField.value;
            }

            protected override void OnReset()
            {
                base.OnReset();
                ScriptInfos = null;
                Directories = null;
                DefaultMatchSettings = null;
                DependencyMaxDepth = 6;
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
        private const string DEPENDENCY_MAX_DEPTH_FIELD_NAME = "script-context-generator__dependecy-max-depth-integer-field";

        private readonly List<ScriptContextScriptInfo> scriptInfos = new();
        private readonly List<string> directories = new();
        private readonly CompositeDisposable bindings = new();
        private readonly CancellationTokenSource destroyCancellationTokenSource = new();
        private readonly Dictionary<VisualElement, CCCompositeDisposable> itemBindings = new();

        [SerializeField] private VisualTreeAsset view = null!;
        [SerializeField] private VisualTreeAsset scriptInfoView = null!;

        private ListView scriptInfosListView = null!;
        private ListView directoriesListView = null!;
        private Button generateButton = null!;
        private Button cancelGenerateButton = null!;
        private MaskField defaultMatchSettingsField = null!;
        private IntegerField dependencyMaxDepthField = null!;

        private bool isGenerating;
        private CancellationTokenSource? disableCancellationTokenSource = new();
        private CancellationTokenSource? cancelGenerateCancellationTokenSource = new();

        private CancellationToken destroyCancellationToken => destroyCancellationTokenSource.Token;
        private CancellationToken disableCancellationToken => disableCancellationTokenSource?.Token ?? default;
        private CancellationToken cancelGenerateCancellationToken => cancelGenerateCancellationTokenSource?.Token ?? default;

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
            dependencyMaxDepthField.value = 6;

            SetupDirectoriesListView();
            SetupScriptInfosListView();

            BindGenerateButton();
            BindCancelGenerateButton();
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
            generateButton = root.QStrict<Button>("script-context-generator__generate-button");
            defaultMatchSettingsField = root.QStrict<MaskField>("script-context-generator__script-name-match-settings");
            dependencyMaxDepthField = root.QStrict<IntegerField>(DEPENDENCY_MAX_DEPTH_FIELD_NAME);
            cancelGenerateButton = root.QStrict<Button>("script-context-generator__cancel-generate-button");
        }

        private void InitStringMatchSettingsField(MaskField maskField)
        {
            var flags = Enum.GetValues(typeof(StringMatchSettings)).Cast<StringMatchSettings>().ToArray();
            maskField.choices = flags.Select(x => x.ToString()).ToList();
            maskField.choicesMasks = flags.Select(x => x.ToIntUnsafe()).ToList();
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

                InitStringMatchSettingsField(matchSettingsField);
                matchSettingsField.value = defaultMatchSettingsField.value;

                void onFieldValueChanged(ChangeEvent<string> ev) => scriptInfos[field.userData.CastTo<int>()].Name = ev.newValue;
                void onMatchSettingsFieldValueChanged(ChangeEvent<int> ev) => scriptInfos[field.userData.CastTo<int>()].MatchSettings = (StringMatchSettings)Enum.ToObject(typeof(StringMatchSettings), ev.newValue);
                void onOnlyAPIToggleValueChanged(ChangeEvent<bool> ev) => scriptInfos[field.userData.CastTo<int>()].OnlyAPI = ev.newValue;
                void onIncludeDependenciesToggleValueChanged(ChangeEvent<bool> ev) => scriptInfos[field.userData.CastTo<int>()].IncludeDependecies = ev.newValue;
                void onRemoveEmptyLinesValueChanged(ChangeEvent<bool> ev) => scriptInfos[field.userData.CastTo<int>()].RemoveEmptyLines = ev.newValue;
                void onDependenciesOnlyApiValueChanged(ChangeEvent<bool> ev) => scriptInfos[field.userData.CastTo<int>()].DependeciesOnlyAPI = ev.newValue;
                void onDependenciesRemoveEmptyLinesValueChanged(ChangeEvent<bool> ev) => scriptInfos[field.userData.CastTo<int>()].DependeciesRemoveEmptyLines = ev.newValue;

                var itemBindings = this.itemBindings.GetOrCreateNew(root);

                field.RegisterValueChangedCallback(onFieldValueChanged);
                CCDisposable.Create(() => field.UnregisterValueChangedCallback(onFieldValueChanged)).AddTo(itemBindings);

                matchSettingsField.RegisterValueChangedCallback(onMatchSettingsFieldValueChanged);
                CCDisposable.Create(() => matchSettingsField.UnregisterValueChangedCallback(onMatchSettingsFieldValueChanged)).AddTo(itemBindings);

                onlyApiToggle.RegisterValueChangedCallback(onOnlyAPIToggleValueChanged);
                CCDisposable.Create(() => onlyApiToggle.UnregisterValueChangedCallback(onOnlyAPIToggleValueChanged)).AddTo(itemBindings);

                includeDependenciesToggle.RegisterValueChangedCallback(onIncludeDependenciesToggleValueChanged);
                CCDisposable.Create(() => includeDependenciesToggle.UnregisterValueChangedCallback(onIncludeDependenciesToggleValueChanged)).AddTo(itemBindings);

                removeEmptyLinesToggle.RegisterValueChangedCallback(onRemoveEmptyLinesValueChanged);
                CCDisposable.Create(() => removeEmptyLinesToggle.UnregisterValueChangedCallback(onRemoveEmptyLinesValueChanged)).AddTo(itemBindings);

                dependenciesOnlyApiToggle.RegisterValueChangedCallback(onDependenciesOnlyApiValueChanged);
                CCDisposable.Create(() => dependenciesOnlyApiToggle.UnregisterValueChangedCallback(onDependenciesOnlyApiValueChanged)).AddTo(itemBindings);

                dependenciesRemoveEmptyLinesToggle.RegisterValueChangedCallback(onDependenciesRemoveEmptyLinesValueChanged);
                CCDisposable.Create(() => dependenciesRemoveEmptyLinesToggle.UnregisterValueChangedCallback(onDependenciesRemoveEmptyLinesValueChanged)).AddTo(itemBindings);

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
                    IncludeDependecies = true,
                    RemoveEmptyLines = true,
                    DependeciesOnlyAPI = true,
                    DependeciesRemoveEmptyLines = true
                });
            }

            void onRemove(BaseListView _)
            {
                bool hasSelected = scriptInfosListView.selectedIndices.IsNotEmpty();
                if (hasSelected)
                {
                    foreach (var index in scriptInfosListView.selectedIndices.OrderByDescending(x => x))
                        scriptInfos.RemoveAt(index);
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
            }

            scriptInfosListView.makeItem += onMakeItem;
            CCDisposable.Create(() => scriptInfosListView.makeItem -= onMakeItem).AddTo(bindings);

            scriptInfosListView.destroyItem += onDestroyItem;
            CCDisposable.Create(() => scriptInfosListView.destroyItem -= onDestroyItem).AddTo(bindings);

            scriptInfosListView.onAdd += onAdd;
            CCDisposable.Create(() => scriptInfosListView.onAdd -= onAdd).AddTo(bindings);

            scriptInfosListView.onRemove += onRemove;
            CCDisposable.Create(() => scriptInfosListView.onRemove -= onRemove).AddTo(bindings);

            scriptInfosListView.bindItem += onBindItem;
            CCDisposable.Create(() => scriptInfosListView.bindItem -= onBindItem).AddTo(bindings);

            scriptInfosListView.itemsSource = scriptInfos;
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

                field.RegisterValueChangedCallback(onFieldValueChanged);
                CCDisposable.Create(() => field.UnregisterValueChangedCallback(onFieldValueChanged)).AddTo(itemBindings);

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

            directoriesListView.makeItem += onMakeItem;
            CCDisposable.Create(() => directoriesListView.makeItem -= onMakeItem).AddTo(bindings);

            directoriesListView.destroyItem += onDestroyItem;
            CCDisposable.Create(() => directoriesListView.destroyItem -= onDestroyItem).AddTo(bindings);

            directoriesListView.onAdd += onAdd;
            CCDisposable.Create(() => directoriesListView.onAdd -= onAdd).AddTo(bindings);

            directoriesListView.onRemove += onRemove;
            CCDisposable.Create(() => directoriesListView.onRemove -= onRemove).AddTo(bindings);

            directoriesListView.bindItem += onBindItem;
            CCDisposable.Create(() => directoriesListView.bindItem -= onBindItem).AddTo(bindings);

            directoriesListView.unbindItem += onUnbindItem;
            CCDisposable.Create(() => directoriesListView.unbindItem -= onUnbindItem).AddTo(bindings);

            directoriesListView.itemsSource = directories;
        }

        private void BindGenerateButton()
        {
            generateButton.clicked += GenerateAndSaveToFile;
            CCDisposable.Create(() => generateButton.clicked -= GenerateAndSaveToFile).AddTo(bindings);
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
            cancelGenerateButton.clicked += CancelGenerate;
            CCDisposable.Create(() => cancelGenerateButton.clicked -= CancelGenerate).AddTo(bindings);
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

                // ✅ Получаем значение глубины из UI
                int maxDepth = dependencyMaxDepthField.value;

                var context = await ScriptContextGenerator.GenerateAsync(
                    scriptInfos,
                    directories.IsNotNullOrEmpty() ? directories : null,
                    dependencyMaxDepth: maxDepth,
                    cancellationToken: cancellationToken);

                var contextDirPath = Path.Combine(EditorLibrary.DataPath, EDITOR_LIBRARY_FOLDER);

                if (!Directory.Exists(contextDirPath))
                    Directory.CreateDirectory(contextDirPath);

                var contextFilePath = Path.Combine(contextDirPath, "Context.txt");
                await File.WriteAllTextAsync(contextFilePath, context.ToString(), cancellationToken);
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
    }
}
#endif