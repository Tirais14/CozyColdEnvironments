#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.FileSystem;
using CCEnvs.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CCEnvs.Json
{
    public static class JsonWordDictionary
    {
        public static async Task<FileEntry> Create(FSPath path,
                                                   string[]? includeNamespaces = null,
                                                   string[]? excludeNamespaces = null)
        {
            Validate.Argument(path, nameof(path), x => x.IsNotDefault());
            Validate.ArgumentNested(path, x => x.HasFileName, nameof(path));
            var directory = new DirectoryEntry(path - path.FileName);

            if (!directory.Exists)
                throw new DirectoryNotFoundException(path);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type[] types = CollectTypes(assemblies, includeNamespaces, excludeNamespaces);
            string[] typeFullnames = CollectTypeFullnames(types);
            string[] fieldNames = CollectFieldNames(types);
            string[] propNames = CollectPropertyNames(types);

            string content = typeFullnames.Concat(fieldNames)
                                          .Concat(propNames)
                                          .Where(x => x.All(c => !char.IsNumber(c)))
                                          .Distinct()
                                          .JoinStringsByLine();

            var file = new FileEntry(path);

            await file.WriteTextAsync(content);
            await file.SaveAsync(overwrite: true);

            return file;
        }

        private static Type[] CollectTypes(Assembly[] assemblies,
                                           string[]? includeNamespaces,
                                           string[]? excludeNamespaces)
        {
            return (from x in assemblies
                    select x.GetTypes() into types
                    from t in types
                    where Filter(t)
                    select t)
                    .ToArray();

            bool Filter(Type type)
            {
                string? ns = type.Namespace;

                if (ns.IsNullOrEmpty())
                {
                    if (includeNamespaces.IsNotNullOrEmpty()
                        &&
                        !includeNamespaces.Contains(string.Empty)
                        ||
                        excludeNamespaces.IsNotNullOrEmpty()
                        &&
                        excludeNamespaces.Contains(string.Empty))
                        return false;

                    return true;
                }

                string[] splittedNS = ns.Split('.');
                if (includeNamespaces.IsNotNullOrEmpty()
                    &&
                    !includeNamespaces.Any(x => splittedNS[0].Equals(x, StringComparison.Ordinal))
                    )
                    return false;

                if (excludeNamespaces.IsNotNullOrEmpty()
                    &&
                    excludeNamespaces.Any(x => splittedNS[0].Equals(x, StringComparison.Ordinal))
                    )
                    return false;

                if (!splittedNS[0].All(x => char.IsLetter(x)))
                    return false;

                return true;
            }
        }

        private static string[] CollectTypeFullnames(Type[] types)
        {
            return (from type in types
                    where !type.IsGenericType
                    let name = type.IsGenericType ? type.Name[..1] : type.Name
                    select GetFullName(type, name) into name
                    where name.All(x => char.IsLetter(x) || x == '.')
                    orderby name
                    select name)
                    .ToArray();

            static string GetFullName(Type type, string name)
            {
                if (type.Namespace.IsNullOrEmpty())
                    return name;

                return type.Namespace + '.' + name;
            }
        }

        private static string[] CollectFieldNames(Type[] types)
        {
            FieldInfo[] fields = types.SelectMany(x => x.ForceGetFields(BindingFlagsDefault.All))
                                      .ToArray();

            var enumFields = new List<FieldInfo>(fields.Where(x => x.ReflectedType.IsEnum));

            return (from field in fields
                    where !field.IsDefined<CompilerGeneratedAttribute>()
                    where field.IsPublic
                          || 
                          field.IsDefined<JsonPropertyAttribute>(inherit: true)
                    where !field.IsInitOnly
                    select field.Name into name
                    select ToCamelCase(name) into name
                    where name.All(x => char.IsLetter(x))
                    orderby name
                    select name)
                    .Concat(enumFields.Select(x => x.Name))
                    .ToArray();
        }

        private static string[] CollectPropertyNames(Type[] types)
        {
            return (from type in types
                    select type.ForceGetProperties(BindingFlagsDefault.All) into props
                    from prop in props
                    where prop.CanWrite
                          ||
                          prop.IsDefined<JsonPropertyAttribute>(inherit: true)
                    where prop.GetAccessors().Any(x => x.IsPublic)
                          ||
                          prop.IsDefined<JsonPropertyAttribute>(inherit: true)
                    select ToCamelCase(prop.Name) into name
                    where name.All(x => char.IsLetter(x))
                    orderby name
                    select name)
                    .ToArray();
        }

        private static string ToCamelCase(string? str)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            int notLetterCount = str.TakeWhile(x => !char.IsLetter(x)).Count();
            str = str[notLetterCount..];

            if (str.Any(x => !char.IsLetter(x) && x != '_')
                ||
                str.All(x => char.IsUpper(x))
                ||
                str.All(x => char.IsLower(x)))
                return str;

            char[] upper = str.TakeWhile(x => char.IsUpper(x))
                              .ToArray();

            if (upper.Length == 1)
                return char.ToLower(str[0]) + str.TrimFirst();
            if (upper.Length > 1)
            {
                upper = upper.Select(x => char.ToLower(x)).ToArray();
                upper = upper.Concat(CC.C.Array(char.ToUpper(str[upper.Length]))).ToArray();
                str = str[(upper.Length + 1)..];

                for (int i = (upper.Length) - (1); i >= 0; i--)
                    str = upper[i] + str;
            }

            return str;
        }
    }
}
