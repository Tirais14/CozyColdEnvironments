using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonConverterHelper
    {
        public static IList<(string propName, object? propValue)> PrepareToSerilize(object obj, JsonSerializerOptions options)
        {
            Guard.IsNotNull(obj);
            Guard.IsNotNull(options);

            Type objType = obj.GetType();
            bool includeFields = options.IncludeFields;
            var results = new List<(string propName, object? propValue)>();

            string propName;
            object? propValue;
            foreach (var member in objType.GetMembers(BindingFlagsDefault.InstanceAll)
                .OrderBy(m => m.GetCustomAttribute<JsonPropertyOrderAttribute>()
                    .Maybe()
                    .Map(m => m.Order)
                    .GetValue(0)))
            {
                if (!IsSerializableMember(member, options))
                    continue;

                if (member is PropertyInfo prop)
                {
                    propValue = prop.GetValue(obj);
                }
                else if (member is FieldInfo field)
                {
                    propValue = field.GetValue(obj);
                }
                else
                    continue;

                propName = GetPropertyName(member, options);
                results.Add((propName, propValue));
            }

            return results;
        }

        /// <exception cref="ArgumentException"></exception>
        public static string GetPropertyName(MemberInfo member, JsonSerializerOptions options)
        {
            Guard.IsNotNull(member);
            Guard.IsNotNull(options);

            var propNameAttribute = (JsonPropertyNameAttribute?)member.GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(JsonPropertyNameAttribute));
            if (propNameAttribute is null)
            {
                if (options.PropertyNamingPolicy is not null)
                    return options.PropertyNamingPolicy.ConvertName(member.Name);
                else
                    return member.Name;
            }
            else
                return propNameAttribute.Name;
        }

        public static bool IsSerializableMember(MemberInfo member, JsonSerializerOptions options)
        {
            Guard.IsNotNull(member);
            Guard.IsNotNull(options);

            if (member.IsDefined<JsonIgnoreAttribute>())
                return false;

            bool hasJsonInclude = member.IsDefined<JsonIncludeAttribute>();
            if (member is PropertyInfo prop)
            {
                if (hasJsonInclude || prop.GetAccessors().Any(x => x.IsPublic))
                    return true;

                return false;
            }  
            else if (member is FieldInfo field)
            {
                if (hasJsonInclude || field.IsPublic)
                    return true;

                return false;
            }

            return false;
        }
    }
}
