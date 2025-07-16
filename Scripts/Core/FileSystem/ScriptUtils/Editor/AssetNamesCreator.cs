using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;
using UTIRLib.Addressables;
using UTIRLib.Attributes;
using UTIRLib.Attributes.Metadata;
using UTIRLib.FileSystem.ScriptUtils;

#nullable enable

namespace UTIRLib.FileSystem.Editor
{
    public sealed class AssetNamesCreator : BaseSharpFileCreator<AddressableAssetSettings>
    {
        private AddressableAssetSettings Settings => values[0];

        public AssetNamesCreator(AddressableAssetSettings value, string[] namespaceParts, bool isEnum = false)
            : base(value, isEnum, namespaceParts)
        {
        }

        public AssetNamesCreator(AddressableAssetSettings value, bool isEnum = false, params string[] namespaceParts)
            : base(value, isEnum, namespaceParts)
        {
        }

        public override ScriptFile[] GetFiles()
        {
            List<ScriptFile> results = new();
            FieldEntry[] fields;
            ScriptFile fileContent;

            Dictionary<string, AddressableAssetEntry[]> groups =
                    AddressableEditorUtility.GatherAllAssetsFromAllGroups(Settings);

            Dictionary<(string groupName, AssetType assetType), AddressableAssetEntry[]> sortedGroups =
                    AddressableEditorUtility.SortAddressablesByGroupAndType(groups);

            string typeName;
            foreach (var group in sortedGroups)
            {
                typeName = GetTypeName(group.Key.groupName, group.Key.assetType);

                fields = CreateFields(group.Value);
                fileContent = CreateFileContent(fields, typeName);

                results.Add(fileContent);
            }

            return results.ToArray();
        }

        private string GetTypeName(string groupName, AssetType assetType)
        {
            string typeName = groupName + assetType.GetMetaString();

            if (IsEnum)
                typeName += "s";

            return typeName;
        }

        private string GetFieldName(AddressableAssetEntry entry)
        {
            if (IsEnum)
            {
                return entry.TargetAsset.name;
            }
            else
                return Syntax.ConvertToConstFieldName(entry.TargetAsset.name);
        }

        private FieldEntry CreateField(AddressableAssetEntry entry)
        {
            string fieldName = GetFieldName(entry);

            if (IsEnum)
            {
                AttributeEntry stringDataAttribute =
                    AttributeFactory.CreateMetaString(entry.TargetAsset.name);

                return new EnumFieldEntry {
                    FieldName = fieldName,
                    Attributes = stringDataAttribute.ToArray()
                };
            }
            else return new FieldEntry<string> {
                AccessModifier = Syntax.AccessModifier.Public,
                FieldName = fieldName,
                FieldValue = entry.TargetAsset.name
            };
        }

        private FieldEntry[] CreateFields(AddressableAssetEntry[] entries)
        {
            const int NONE_FIELD_OFFSET = 1;

            FieldEntry[] fields;
            if (IsEnum)
            {
                fields = new FieldEntry[entries.Length + NONE_FIELD_OFFSET];
                fields[0] = EnumFieldFactory.CreateNone();
            }
            else
                fields = new FieldEntry[entries.Length];

            for (int i = 0; i < entries.Length; i++)
                fields[IsEnum ? i + NONE_FIELD_OFFSET : i] = CreateField(entries[i]);

            return fields;
        }

        private ScriptFile CreateFileContent(IField[] fields, string typeName)
        {
            IType typeData;

            if (IsEnum)
                typeData = new EnumEntry {
                    AccessModifier = Syntax.AccessModifier.Public,
                    TypeName = typeName,
                    Fields = fields.Cast<EnumFieldEntry>().ToArray()
                };
            else 
                typeData = new ClassEntry {
                AccessModifier = Syntax.AccessModifier.Public,
                OtherModifiers = Syntax.OtherModifiers.Static,
                TypeName = typeName,
                Members = fields
            };

            var file = new ScriptFile{
                Name = typeName,
            };

            file.SetContent(typeData);

            return file;
        }
    }
}