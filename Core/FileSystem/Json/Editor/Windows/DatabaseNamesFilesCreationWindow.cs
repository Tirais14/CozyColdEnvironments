using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UTIRLib.Diagnostics;
using UTIRLib.Editor;
using UTIRLib.FileSystem;
using UTIRLib.FileSystem.ScriptUtils;

#nullable enable
namespace UTIRLib.Json.Editor
{
    public abstract class DatabaseNamesFilesCreationWindow : TirLibEditorWindow
    {
        [Flags]
        protected enum FileTypes
        {
            None,
            Const,
            Enum
        }

        protected Toggle overwriteToggle = null!;
        protected EnumFlagsField fileTypesToCreateField = null!;
        protected TextField constFileName = null!;
        protected TextField enumFileName = null!;
        protected Button createButton = null!;

        protected virtual string ConstFileName => "ConstDatabaseName";
        protected virtual string EnumFileName => "EnumDatabaseName";
        protected abstract NamespaceEntry NamespaceData { get; }

        protected override void ConstructUIElements()
        {
            base.ConstructUIElements();

            overwriteToggle ??= new Toggle("Overwrite Files") {
                value = true
            };

            fileTypesToCreateField = new EnumFlagsField("File types to create", default(FileTypes));

            constFileName = new TextField("Const filename") {
                value = ConstFileName
            };
            enumFileName = new TextField("Enum filename") {
                value = EnumFileName
            };

            createButton ??= new Button(OnCreateClick) {
                text = "Create Database Names Files"
            };
        }

        protected void OnCreateClick()
        {
            if (fileTypesToCreateField.value.Equals(FileTypes.None))
            {
                Abort("File types to create not selected.");
                return;
            }

            bool toRefresh = false;
            if (fileTypesToCreateField.value.HasFlag(FileTypes.Const))
            {
                if (constFileName.value.IsNullOrWhiteSpace())
                    TirLibDebug.Warning("Specify the const file name.", this);
                else
                {
                    CreateClassFile();
                    toRefresh = true;
                }
            }



            if (fileTypesToCreateField.value.HasFlag(FileTypes.Enum))
            {
                if (enumFileName.value.IsNullOrWhiteSpace())
                    TirLibDebug.Warning("Specify the enum file name.", this);
                else
                {
                    CreateEnumFile();
                    toRefresh = true;
                }
            }

            if (toRefresh)
                AssetDatabase.Refresh();
        }

        protected virtual AddressableAssetSettings GetAddressableSettings()
        {
            return AddressableAssetSettingsDefaultObject.Settings;
        }

        protected virtual FSPath SelectConstFileDirectoryPath()
        {
            return SelectDirectory(title: "Select const file directory path");
        }

        protected virtual FSPath SelectEnumFileDirectoryPath()
        {
            return SelectDirectory(title: "Select enum file directory path");
        }

        private FieldEntry<string>[] GetClassFields()
        {
            List<FieldEntry<string>> fields = new();

            IEnumerable<string> groupNames = GetAddressableSettings().groups.Select(x => x.Name);

            FieldEntry<string> field = new(){
                AccessModifier = Syntax.AccessModifier.Public,
                OtherModifierFlags = Syntax.OtherModifiers.Const
            };
            foreach (var groupName in groupNames)
            {
                fields.Add(field with {
                    FieldName = Syntax.ConvertToConstFieldName(groupName),
                    FieldValue = groupName
                });
            }

            return fields.ToArray();
        }

        private ClassEntry GetClass()
        {
            return new ClassEntry {
                AccessModifier = Syntax.AccessModifier.Public,
                OtherModifiers = Syntax.OtherModifiers.Static,
                TypeName = constFileName.value,
                Members = GetClassFields(),
            };
        }

        private void CreateClassFile()
        {
            ScriptContentList fileContent = ScriptFileBuilder.BuildContent(
                NamespaceData.ToArray(),
                GetClass().ToArray());

            FSPath savePath = SelectConstFileDirectoryPath();

            savePath += constFileName.value;

            if (TryAbortByFilePath(savePath))
                return;

            ScriptFile file = ScriptFileBuilder.Build(savePath, fileContent);

            file.Save(overwriteToggle.value);
        }

        private EnumFieldEntry[] GetEnumFields()
        {
            List<EnumFieldEntry> fields = new();

            EnumFieldEntry field = new(){
                TabulationsCount = 2
            };
            IEnumerable<string> groupNames = GetAddressableSettings().groups.Select(x => x.Name);
            foreach (var groupName in groupNames)
            {
                fields.Add(field with {
                    FieldName = groupName.DeleteWhitespaces(),
                    Attributes = AttributeFactory.CreateMetaString(groupName).ToArray()
                });
            }

            return fields.ToArray();
        }

        private EnumEntry GetEnum()
        {
            return new EnumEntry {
                AccessModifier = Syntax.AccessModifier.Public,
                TypeName = enumFileName.value,
                ParentType = EnumType.Ulong,
                Fields = GetEnumFields(),
            };
        }

        private void CreateEnumFile()
        {
            ScriptContentList fileContent = ScriptFileBuilder.BuildContent(
                NamespaceData.ToArray(),
                GetEnum().ToArray());

            FSPath savePath = SelectEnumFileDirectoryPath();

            savePath += enumFileName.value;

            if (TryAbortByFilePath(savePath))
                return;

            ScriptFile file = ScriptFileBuilder.Build(savePath, fileContent);

            file.Save(overwriteToggle.value);
        }
    }
}
