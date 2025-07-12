using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UTIRLib.Linq;

#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public static class UsingHelper
    {
        public static string[] DistinctUsings(string[] contentLines, string[] usings)
        {
            if (usings.IsNullOrEmpty())
                return contentLines;

            string[] contentUsings = GetUsingLines(contentLines);

            //Except usings from content
            contentLines = contentLines.Except(contentUsings).ToArray();

            //Concat usings, ordering and deletes duplicates
            contentUsings = contentUsings.Concat(usings).Distinct().OrderBy(x => x).ToArray();

            //Concat filtered usings and content
            return contentUsings.Concat(contentLines).ToArray();
        }

        public static string[] GetUsingLines(string[] lines)
        {
            return lines.Where(x => Regex.IsMatch(x, @"^using\s+[\w.]+;\s*$")).ToArray();
        }

        public static UsingEntry[] GetUsings(string[] lines)
        {
            string[] usingLines = GetUsingLines(lines);
            var results = new List<UsingEntry>(usingLines.Length);

            string namespaceValue;
            for (int i = 0; i < usingLines.Length; i++)
            {
                namespaceValue = usingLines[i].Delete("using").DeleteWhitespaces();

                results.Add(new UsingEntry {
                    NamespaceValue = namespaceValue
                });
            }

            return results.ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static UsingEntry Resolve(Type forType)
        {
            if (forType == null)
                throw new ArgumentNullException(nameof(forType));

            return new UsingEntry(forType.Namespace);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static UsingEntry Resolve(object obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            return Resolve(obj.GetType());
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static UsingEntry[] Resolve(object[] values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));
            if (values.IsEmpty())
                return Array.Empty<UsingEntry>();

            var usings = new UsingEntry[values.Length];
            for (int i = 0; i < values.Length; i++)
                usings[i] = Resolve(values[i]);

            return usings;
        }
        public static UsingEntry? Resolve(ITypeProvider typeProvider)
        {
            if (typeProvider.TypeValue is null)
                return null;

            return Resolve(typeProvider.TypeValue);
        }
        public static UsingEntry[] Resolve(IEnumerable<ITypeProvider> typeProvider)
        {
            List<UsingEntry> usings = new();

            UsingEntry? created;
            foreach (var item in typeProvider)
            {
                created = Resolve(item);

                if (created is not null)
                    usings.Add(created);
            }

            return usings.ToArray();
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static UsingEntry[] Resolve(IScriptContent scriptContent,
                                           bool distinctResults = true)
        {
            if (scriptContent is null)
                throw new ArgumentNullException(nameof(scriptContent));

            var usings = new List<UsingEntry>();

            UsingEntry usingEntry;
            foreach (var provider in scriptContent.ToCustomEnumerable<ITypeProvider>())
            {
                if (provider.TypeValue != null)
                {
                    usingEntry = Resolve(provider.TypeValue);

                    usings.Add(usingEntry);
                }
            }

            foreach (var provider in scriptContent.ToCustomEnumerable<IUsingsProvider>())
                usings.AddRange(provider.Usings);

            foreach (var type in scriptContent.ToCustomEnumerable<IType>())
            {
                for (int i = 0; i < type.ParentTypes.Length; i++)
                    usings.Add(Resolve(type.ParentTypes[i]));
            }

            if (distinctResults)
                return Distinct(usings);

            return usings.ToArray();
        }
        public static UsingEntry[] Resolve(IEnumerable<IScriptContent> lines,
                                           bool distinctResults = true)
        {
            var results = new List<UsingEntry>();

            UsingEntry[] usings;
            foreach (var line in lines)
            {
                usings = Resolve(line);

                results.AddRange(usings);
            }

            if (distinctResults)
                return Distinct(results);

            return results.ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static UsingEntry[] Distinct(IEnumerable<UsingEntry> usings)
        {
            if (usings is null)
                throw new ArgumentNullException(nameof(usings));

            var results = new List<UsingEntry>();
            foreach (var entry in usings)
            {
                if (!results.Exists(x => x.ToString() == entry.ToString()))
                    results.Add(entry);
            }

            return results.ToArray();
        }
    }
}
