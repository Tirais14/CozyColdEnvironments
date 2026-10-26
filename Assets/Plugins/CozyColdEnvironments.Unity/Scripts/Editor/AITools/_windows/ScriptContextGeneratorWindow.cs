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
            [DataMember(Name = "namespaces")]
            public ScriptContextNamespace[]? Namespaces { get; set; }

            [DataMember(Name = "namespaceMatchSettings")]
            public StringMatchSettings? NamespaceMatchSettings { get; set; }

            public Snapshot(Snapshot<ScriptContextGeneratorWindow> original) : base(original)
            {
            }

            public Snapshot()
            {
            }

            public Snapshot(ScriptContextGeneratorWindow target) : base(target)
            {
            }

            protected override void OnRestore(ref ScriptContextGeneratorWindow target)
            {
                if (Namespaces is not null)
                    target.namespaces.AddRange(Namespaces);

                if (NamespaceMatchSettings.HasValue)
                    target.namespaceMatchSettingsField.value = (int)NamespaceMatchSettings.Value;
            }

            protected override void OnCapture(ScriptContextGeneratorWindow target)
            {
                base.OnCapture(target);
                Namespaces = target.namespaces.ToArray();
                NamespaceMatchSettings = (StringMatchSettings)Enum.ToObject(typeof(StringMatchSettings), target.namespaceMatchSettingsField.value);
            }

            protected override void OnReset()
            {
                base.OnReset();
                Namespaces = null;
                NamespaceMatchSettings = null;
            }
        }

        private const string EDITOR_LIBRARY_FOLDER = "ScriptContextGeneratorWindow";
        private const string SAVE_PATH = EDITOR_LIBRARY_FOLDER + "/data.json";
        private const string NAMESPACE_FIELD_NAME = "script-context-generator__namespace";
        private const string NAMESPACE_MATCH_SETTINGS_FIELD_NAME = "script-context-generator__namespace-match-settings";
        private const string NAMESPACE_ONLY_API_TOGGLE_NAME = "script-context-generator__only-api";
        private const string NAMESPACE_INCLUDE_DEPENDECIES_TOGGLE_NAME = "script-context-generator__include-dependecies";
        private const string NAMESPACE_REMOVE_EMPTY_LINES_TOGGLE_NAME = "script-context-generator__remove-empty-lines";
        private const string NAMESPACE_DEPENDENCIES_ONLY_API_TOGGLE_NAME = "script-context-generator__dependecies-only-api";
        private const string NAMESPACE_DEPENDENCIES_REMOVE_EMPTY_LINES_TOGGLE_NAME = "script-context-generator__dependecies-remove-empty-lines";

        private readonly List<ScriptContextNamespace> namespaces = new();

        private readonly CompositeDisposable bindings = new();

        private readonly CancellationTokenSource destroyCancellationTokenSource = new();

        private readonly Dictionary<VisualElement, CCCompositeDisposable> itemBindings = new();

        private readonly List<string> directories = new();

        [SerializeField]
        private VisualTreeAsset view = null!;

        [SerializeField]
        private VisualTreeAsset namespaceView = null!;

        private ListView namespacesListView = null!;
        private ListView directoriesListView = null!;

        private Button generateButton = null!;
        private Button cancelGenerateButton = null!;

        private MaskField namespaceMatchSettingsField = null!;

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
                this.PrintError(DebugMessageBuilder.CreatePooled()
                    .AddMessage("View is null")
                    .ToStringAndDispose()
                    );
                return;
            }    

            var root = rootVisualElement;
            InitRootElement(root);
            InitStringMatchSettingsField(namespaceMatchSettingsField);
            namespaceMatchSettingsField.value = (int)StringMatchSettings.Ordinal;
            SetupNamepsacesListView();
            BindGenerateButton();
            BindCancelGenerateButton();
            RestoreSavedData();
        }

        private void OnEnable()
        {
            disableCancellationTokenSource?.CancelAndDispose();
            disableCancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            SaveData();
        }

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
        public static void ShowExample()
        {
            GetWindow<ScriptContextGeneratorWindow>("Script Context Generator");
        }

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
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }

        private void InitRootElement(VisualElement root)
        {
            root.Add(view.Instantiate());

            namespacesListView = root.QStrict<ListView>("script-context-generator__namespaces-list-view");
            generateButton = root.QStrict<Button>("script-context-generator__generate-button");
            namespaceMatchSettingsField = root.QStrict<MaskField>("script-context-generator__namespace-match-settings");
            cancelGenerateButton = root.QStrict<Button>("script-context-generator__cancel-generate-button");
        }

        private void InitStringMatchSettingsField(MaskField maskField)
        {
            var flags = Enum.GetValues(typeof(StringMatchSettings)).Cast<StringMatchSettings>().ToArray();

            maskField.choices = flags.Select(x => x.ToString()).ToList();
            maskField.choicesMasks = flags.Select(x => x.ToIntUnsafe()).ToList();
        }

        private void SetupNamepsacesListView()
        {
            VisualElement onMakeItem()
            {
                var root = namespacesListView.itemTemplate.Instantiate();
                var field = root.QStrict<TextField>(NAMESPACE_FIELD_NAME);
                var matchSettingsField = root.QStrict<MaskField>(NAMESPACE_MATCH_SETTINGS_FIELD_NAME);
                var onlyApiToggle = root.QStrict<Toggle>(NAMESPACE_ONLY_API_TOGGLE_NAME);
                var includeDependeciesToggle = root.QStrict<Toggle>(NAMESPACE_INCLUDE_DEPENDECIES_TOGGLE_NAME);

                // ✅ НОВОЕ: Получаем ссылки на новые Toggle
                var removeEmptyLinesToggle = root.QStrict<Toggle>(NAMESPACE_REMOVE_EMPTY_LINES_TOGGLE_NAME);
                var dependenciesOnlyApiToggle = root.QStrict<Toggle>(NAMESPACE_DEPENDENCIES_ONLY_API_TOGGLE_NAME);
                var dependenciesRemoveEmptyLinesToggle = root.QStrict<Toggle>(NAMESPACE_DEPENDENCIES_REMOVE_EMPTY_LINES_TOGGLE_NAME);

                InitStringMatchSettingsField(matchSettingsField);
                matchSettingsField.value = namespaceMatchSettingsField.value;

                void onFieldValueChanged(ChangeEvent<string> ev)
                {
                    namespaces[field.userData.CastTo<int>()].Name = ev.newValue;
                }

                void onMatchSettingsFieldValueChanged(ChangeEvent<int> ev)
                {
                    namespaces[field.userData.CastTo<int>()].MatchSettings = (StringMatchSettings)Enum.ToObject(typeof(StringMatchSettings), ev.newValue);
                }

                void onOnlyAPIToggleValueChanged(ChangeEvent<bool> ev)
                {
                    namespaces[field.userData.CastTo<int>()].OnlyAPI = ev.newValue;
                }

                void onIncludeDependeciesToggleValueChanged(ChangeEvent<bool> ev)
                {
                    namespaces[field.userData.CastTo<int>()].IncludeDependecies = ev.newValue;
                }

                // ✅ НОВОЕ: Обработчики изменений для новых полей
                void onRemoveEmptyLinesValueChanged(ChangeEvent<bool> ev)
                {
                    namespaces[field.userData.CastTo<int>()].RemoveEmptyLines = ev.newValue;
                }

                void onDependenciesOnlyApiValueChanged(ChangeEvent<bool> ev)
                {
                    namespaces[field.userData.CastTo<int>()].DependeciesOnlyAPI = ev.newValue;
                }

                void onDependenciesRemoveEmptyLinesValueChanged(ChangeEvent<bool> ev)
                {
                    namespaces[field.userData.CastTo<int>()].DependeciesRemoveEmptyLines = ev.newValue;
                }

                var itemBindings = this.itemBindings.GetOrCreateNew(root);

                field.RegisterValueChangedCallback(onFieldValueChanged);
                CCDisposable.Create(() => field.UnregisterValueChangedCallback(onFieldValueChanged)).AddTo(itemBindings);

                matchSettingsField.RegisterValueChangedCallback(onMatchSettingsFieldValueChanged);
                CCDisposable.Create(() => matchSettingsField.UnregisterValueChangedCallback(onMatchSettingsFieldValueChanged)).AddTo(itemBindings);

                onlyApiToggle.RegisterValueChangedCallback(onOnlyAPIToggleValueChanged);
                CCDisposable.Create(() => onlyApiToggle.UnregisterValueChangedCallback(onOnlyAPIToggleValueChanged)).AddTo(itemBindings);

                includeDependeciesToggle.RegisterValueChangedCallback(onIncludeDependeciesToggleValueChanged);
                CCDisposable.Create(() => includeDependeciesToggle.UnregisterValueChangedCallback(onIncludeDependeciesToggleValueChanged)).AddTo(itemBindings);

                // ✅ НОВОЕ: Регистрация и отписка новых обработчиков
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
                if (itemBindings.TryGetValue(item, out var bindings))
                    bindings.Dispose();
            }

            void onAdd(BaseListView list)
            {
                namespaces.Add(new ScriptContextNamespace()
                {
                    Name = string.Empty,
                    MatchSettings = (StringMatchSettings)Enum.ToObject(typeof(StringMatchSettings), namespaceMatchSettingsField.value),
                    OnlyAPI = false,
                    IncludeDependecies = true,
                    // ✅ НОВОЕ: Можно задать значения по умолчанию при создании, или оставить false (по умолчанию для bool)
                    RemoveEmptyLines = true,
                    DependeciesOnlyAPI = true,
                    DependeciesRemoveEmptyLines = true
                });
            }

            void onRemove(BaseListView _)
            {
                bool hasSelected = namespacesListView.selectedIndices.IsNotEmpty();

                if (hasSelected)
                {
                    foreach (var index in namespacesListView.selectedIndices.OrderByDescending(x => x))
                        namespaces.RemoveAt(index);
                }
                else
                    namespaces.RemoveAt(namespaces.Count - 1);

                namespacesListView.RefreshItems();
            }

            void onBindItem(VisualElement item, int index)
            {
                var field = item.QStrict<TextField>(NAMESPACE_FIELD_NAME);
                var matchSettingsField = item.QStrict<MaskField>(NAMESPACE_MATCH_SETTINGS_FIELD_NAME);
                var onlyApiToggle = item.QStrict<Toggle>(NAMESPACE_ONLY_API_TOGGLE_NAME);
                var includeDependeciesToggle = item.QStrict<Toggle>(NAMESPACE_INCLUDE_DEPENDECIES_TOGGLE_NAME);

                // ✅ НОВОЕ: Получаем ссылки на новые Toggle для привязки данных
                var removeEmptyLinesToggle = item.QStrict<Toggle>(NAMESPACE_REMOVE_EMPTY_LINES_TOGGLE_NAME);
                var dependenciesOnlyApiToggle = item.QStrict<Toggle>(NAMESPACE_DEPENDENCIES_ONLY_API_TOGGLE_NAME);
                var dependenciesRemoveEmptyLinesToggle = item.QStrict<Toggle>(NAMESPACE_DEPENDENCIES_REMOVE_EMPTY_LINES_TOGGLE_NAME);

                if (index >= namespaces.Count)
                {
                    throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                        .AddMessage("Not found namespace at index")
                        .AddProperty("Index", index)
                        .ToStringAndDispose()
                        );
                }

                var nsContext = namespaces[index];

                field.userData = index;
                field.value = nsContext.Name;
                matchSettingsField.value = (int)nsContext.MatchSettings;
                onlyApiToggle.value = nsContext.OnlyAPI;
                includeDependeciesToggle.value = nsContext.IncludeDependecies;

                // ✅ НОВОЕ: Привязка значений из модели к UI
                removeEmptyLinesToggle.value = nsContext.RemoveEmptyLines;
                dependenciesOnlyApiToggle.value = nsContext.DependeciesOnlyAPI;
                dependenciesRemoveEmptyLinesToggle.value = nsContext.DependeciesRemoveEmptyLines;
            }

            void onUnbindItem(VisualElement item, int index)
            {
                // Оставляем пустым, так как очистка происходит в onDestroyItem, 
                // а значения перезаписываются в onBindItem
            }

            namespacesListView.makeItem += onMakeItem;
            CCDisposable.Create(() => namespacesListView.makeItem -= onMakeItem).AddTo(bindings);

            namespacesListView.destroyItem += onDestroyItem;
            CCDisposable.Create(() => namespacesListView.destroyItem -= onDestroyItem).AddTo(bindings);

            namespacesListView.onAdd += onAdd;
            CCDisposable.Create(() => namespacesListView.onAdd -= onAdd).AddTo(bindings);

            namespacesListView.onRemove += onRemove;
            CCDisposable.Create(() => namespacesListView.onRemove -= onRemove).AddTo(bindings);

            namespacesListView.bindItem += onBindItem;
            CCDisposable.Create(() => namespacesListView.bindItem -= onBindItem).AddTo(bindings);

            namespacesListView.unbindItem += onUnbindItem;
            CCDisposable.Create(() => namespacesListView.unbindItem -= onUnbindItem).AddTo(bindings);

            namespacesListView.itemsSource = namespaces;
        }

        private async void GenerateAndSaveToFile()
        {
            if (isGenerating)
                return;

            isGenerating = true;
            generateButton.enabledSelf = false;
            cancelGenerateButton.enabledSelf = true;

            try
            {
                using var _ = disableCancellationToken.TryLinkTokens(cancelGenerateCancellationToken, out var cancellationToken);
                var context = await ScriptContextGenerator.GenerateAsync(namespaces, Range.From("*"), cancellationToken: cancellationToken);
                var contextDirPath = Path.Combine(EditorLibrary.DataPath, EDITOR_LIBRARY_FOLDER);

                if (!Directory.Exists(contextDirPath))
                    Directory.CreateDirectory(contextDirPath);

                var contextFilePath = Path.Combine(contextDirPath, "Context.txt");
                await File.WriteAllTextAsync(contextFilePath, context.ToString(), cancellationToken);
            }
            catch(Exception ex)
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

        private void BindGenerateButton()
        {
            generateButton.clicked += GenerateAndSaveToFile;
            CCDisposable.Create(() => generateButton.clicked -= GenerateAndSaveToFile)
                .AddTo(bindings);
        }

        private void CancelGenerate()
        {
            if (!isGenerating)
                return;

            cancelGenerateCancellationTokenSource?.CancelAndDispose();
            cancelGenerateCancellationTokenSource = new CancellationTokenSource();

            isGenerating = false;
            generateButton.enabledSelf = true;
            cancelGenerateButton.enabledSelf = false;
        }

        private void BindCancelGenerateButton()
        {
            cancelGenerateButton.clicked += CancelGenerate;
            CCDisposable.Create(() => cancelGenerateButton.clicked -= CancelGenerate)
                .AddTo(bindings); ;
        }
    }
}
#endif