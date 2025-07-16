#nullable enable
using System;
using System.Collections.Generic;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public static class ScriptFileBuilder
    {
        /// <param name="resolveUsings">if true, uses <see cref="UsingHelper"/> for resolve</param>
        public static ScriptContentList BuildContent(
            this IEnumerable<IScriptContent> contentLines,
            bool resolveUsings = true)
        {
            UsingEntry[] usings = GetUsings(contentLines, resolveUsings);

            NamespaceEntry? namespaceEntry = contentLines.ExtractNamespace();

            IType[] types = contentLines.ExtractTypes();

            var fileContent = new ScriptContentList(usings.Length + (namespaceEntry is not null ? 1 : types.Length));

            fileContent.AddRange(usings);
            if (namespaceEntry is not null)
            {
                namespaceEntry.Content = types;
                fileContent.Add(namespaceEntry);
            }
            else
                fileContent.AddRange(types);

            return fileContent;
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static ScriptContentList BuildContent(
            bool resolveUsings,
            params IEnumerable<IScriptContent>[] contentParts)
        {
            if (contentParts is null)
                throw new ArgumentNullException(nameof(contentParts));
            if (contentParts.IsEmpty())
                return new ScriptContentList();

            var contentLines = new List<IScriptContent>();
            contentParts.ForEach(x => contentLines.AddRange(x));

            return BuildContent(contentLines, resolveUsings);
        }
        public static ScriptContentList BuildContent(
            params IEnumerable<IScriptContent>[] contentParts)
        {
            return BuildContent(resolveUsings: true, contentParts);
        }

        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static ScriptFile Build(FSPath pathWithFileName, ScriptContentList fileContent)
        {
            if (!pathWithFileName.IsValid)
                throw new ArgumentException("Path is not valid.");
            if (fileContent is null)
                throw new ArgumentNullException(nameof(fileContent));

            var file = new ScriptFile(pathWithFileName);

            file.SetContent(fileContent);

            return file;
        }

        private static UsingEntry[] GetUsings(IEnumerable<IScriptContent> contentLines,
                                              bool resolveUsings)
        {
            if (resolveUsings)
                return UsingHelper.Resolve(contentLines, distinctResults: true);

            return contentLines.ExtractUsings();
        }
    }
}
