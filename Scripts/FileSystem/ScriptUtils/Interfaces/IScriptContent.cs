using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public interface IScriptContent
    {
        int TabulationsCount { get; set; }
    }

    public static class IScriptContentExtensions
    {
        /// <summary>
        /// For enumerating all nested <see cref="IScriptContent"/>, without recursion
        /// </summary>
        public static ScriptContentEnumerable<T> ToCustomEnumerable<T>(
            this IScriptContent scriptContent)
        {
            return new ScriptContentEnumerable<T>(scriptContent);
        }

        /// <summary>
        /// For enumerating all nested <see cref="IScriptContent"/>, without recursion
        /// </summary>
        public static ScriptContentEnumerable<T> ToCustomEnumerable<T>(
            this IEnumerable<IScriptContent> scriptLines)
        {
            return new ScriptContentEnumerable<T>(scriptLines);
        }

        public static T[] ToArray<T>(this T value)
            where T : IScriptContent
        {
            return new T[] { value };
        }

        public static string[] ToStringArray(
            this IEnumerable<IScriptContent> contentLines)
        {
            return contentLines.Select(x => x.ToString()).ToArray();
        }

        public static NamespaceEntry[] ExtractNamespaces(
            this IEnumerable<IScriptContent> contentLines)
        {
            return contentLines.OfType<NamespaceEntry>().ToArray();
        }

        public static NamespaceEntry? ExtractNamespace(
            this IEnumerable<IScriptContent> contentLines)
        {
            NamespaceEntry[] namespaces = contentLines.ExtractNamespaces();

            return namespaces.FirstOrDefault();
        }

        public static UsingEntry[] ExtractUsings(
            this IEnumerable<IScriptContent> contentLines)
        {
            return contentLines.OfType<UsingEntry>().ToArray();
        }

        public static IType[] ExtractTypes(
            this IEnumerable<IScriptContent> contentLines)
        {
            return contentLines.OfType<IType>().ToArray();
        }

        public static ITypeMember[] ExtractTypeMembers(
            this IEnumerable<IScriptContent> contentLines)
        {
            return contentLines.OfType<ITypeMember>().ToArray();
        }

        public static ConstructorEntry[] ExtractConstructors(
            this IEnumerable<IScriptContent> contentLines)
        {
            return contentLines.OfType<ConstructorEntry>().ToArray();
        }

        public static T[] ExtractFields<T>(
            this IEnumerable<IScriptContent> contentLines)
            where T : IField
        {
            return contentLines.OfType<T>().ToArray();
        }

        public static IProperty[] ExtractProperties(
            this IEnumerable<IScriptContent> contentLines)
        {
            return contentLines.OfType<IProperty>().ToArray();
        }

        public static IMethod[] ExtractMethods<T>(
            this IEnumerable<IScriptContent> contentLines)
            where T : IMethod
        {
            return contentLines.OfType<IMethod>().ToArray();
        }
    }
}
