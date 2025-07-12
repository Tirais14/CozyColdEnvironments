using System.Collections.Generic;
using System.IO;
using System.Linq;
using UTIRLib.FileSystem.ScriptUtils;
using UTIRLib.Utils;

#nullable enable

namespace UTIRLib.FileSystem.Editor
{
    public sealed class DirectoryInfoFilesCreator : BaseSharpFileCreator<string>
    {
        private readonly string[]? excludeDirectories;
        private readonly bool excludeByFullName;

        public DirectoryInfoFilesCreator(
            string[] directories,
            string[]? excludeDirectories,
            bool excludeByFullName,
            bool isEnum,
            params string[] namespaceParts)
            : base(directories,
                   isEnum,
                   namespaceParts)
        {
            this.excludeDirectories = excludeDirectories;
            this.excludeByFullName = excludeByFullName;
        }

        public DirectoryInfoFilesCreator(
            string directory,
            string[]? excludeDirectories,
            bool excludeByFullName,
            bool isEnum,
            params string[] namespaceParts)
            : base(directory,
                   isEnum,
                   namespaceParts)
        {
            this.excludeDirectories = excludeDirectories;
            this.excludeByFullName = excludeByFullName;
        }

        public override ScriptFile[] GetFiles()
        {
            List<ScriptFile> files = new();

            ScriptFile fileData;
            IField[] fields;

            string[] childDirectories;
            string filename;
            foreach (var directory in values)
            {
                childDirectories = Directory.GetDirectories(directory, "*", SearchOption.AllDirectories).
                                   Select(x => FSPathHelper.SetStyle(x, PathStyle.Universal)).
                                   ToArray();

                if (excludeDirectories.IsNotNullOrEmpty())
                {
                    childDirectories = FilterDirectoreis(childDirectories);
                }

                fields = CreateFields(childDirectories, directory);

                filename = GetFileName(directory);

                fileData = GetFileData(fields, filename);
                files.Add(fileData);
            }

            return files.ToArray();
        }

        private string[] FilterDirectoreis(string[] directories)
        {
            if (excludeByFullName) return directories.Where(a => !excludeDirectories.Any(b => a.Equals(b))).ToArray();
            else return directories.Where(a => !excludeDirectories.Any(b => a.Contains(b))).ToArray();
        }

        private string GetFileName(string directory)
        {
            if (IsEnum) return (Path.GetFileName(directory) + "Directories").DeleteWhitespaces();
            else return (Path.GetFileName(directory) + "Directory").DeleteWhitespaces();
        }

        private IField[] CreateFields(string[] childDirectories, string parentDirectory)
        {
            const int OFFSET = 1;
            List<IField> fields = new(childDirectories.Length + OFFSET) {
                CreateField(parentDirectory, "Path")
            };

            foreach (var childDirectory in childDirectories)
            {
                string fieldName = GetFieldName(childDirectory, parentDirectory);

                IField field = CreateField(childDirectory, fieldName);

                fields.Add(field);

                if (!IsEnum && ResourcesHelper.IsInResourcesDirectory(childDirectory))
                {
                    string resourcesRelativeDirectoryPath = ResourcesHelper.GetRelativePath(childDirectory);

                    field = CreateField(resourcesRelativeDirectoryPath, fieldName + "_Relative");

                    fields.Add(field);
                }
            }

            return fields.ToArray();
        }

        private static string GetFieldName(string directory, string? parentDirectory = null)
        {
            var relativePath = parentDirectory.IsNotNullOrEmpty()
                               ? Path.GetRelativePath(parentDirectory, directory)
                               : directory;

            return relativePath.Replace(FSPath.Separators, '_').DeleteWhitespaces();
        }

        //private TypeData CreateEnumExtensionsClass(string enumTypeName)
        //{
        //    StringBuilder methodBody = new();
        //    methodBody.AppendLine("{");
        //    methodBody.AppendLine($"string path = value.{nameof(EnumExtensions.GetStringValue)}<{enumTypeName}>()");
        //    methodBody.AppendLine();
        //    methodBody.AppendLine($"return path");
        //    methodBody.AppendLine("}");

        //    MethodData method = new MethodData<string>() {
        //        Name = "GetPath",
        //        RawArguments = ((object)new ArgumentData(enumTypeName,
        //                                                 "value",
        //                                                 ArgumentData.Settings.Extension)).AsArray()
        //    };

        //    return new TypeData() {
        //        Keywords = Keywords.Public | Keywords.Static | Keywords.Class,
        //        Name = enumTypeName + "Extensions",
        //        RawValue = method.AsArray()
        //    };
        //}

        private IField CreateField(string directory, string fieldName)
        {
            if (IsEnum)
            {
                AttributeEntry metadataAttribute = AttributeFactory.CreateMetaString(directory);

                return new EnumFieldEntry {
                    FieldName = fieldName,
                    Attributes = metadataAttribute.ToArray()
                };
            }
            else
                return new FieldEntry<string> {
                    AccessModifier = Syntax.AccessModifier.Public,
                    FieldName = fieldName,
                    FieldValue = directory
                };
        }

        private ScriptFile GetFileData(IField[] fields, string typeName)
        {
            IType typeData = IsEnum switch {
                true => new EnumEntry {
                    AccessModifier = Syntax.AccessModifier.Public,
                    TypeName = typeName,
                    Fields = fields.Cast<EnumFieldEntry>().ToArray()
                },
                _ => new ClassEntry {
                    AccessModifier = Syntax.AccessModifier.Public,
                    OtherModifiers = Syntax.OtherModifiers.Static,
                    TypeName = typeName,
                    Members = fields
                },
            };

            ScriptFile file = new(typeName){
                Name = typeName
            };

            file.SetContent(typeData);

            return file;
        }
    }
}