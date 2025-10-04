using CCEnvs.Reflection;
using System;
using System.Linq;

#nullable enable

namespace CCEnvs.Attributes.Metadata
{
    public static class EnumExtensions
    {
        public static string GetMetaString<T>(this T value, bool throwIfNotFound = true)
            where T : Enum
        {
            return value.GetFieldInfo()
                        .GetMetadata<MetaStringAttribute>(throwIfNotFound)
                        .Single().Value;
        }

        public static Type GetMetaType<T>(this T value, bool throwIfNotFound = true)
            where T : Enum
        {
            return value.GetFieldInfo()
                        .GetMetadata<MetaTypeAttribute>(throwIfNotFound)
                        .Single().Value;
        }

        public static string[] GetMetaStrings<T>(this T value,
                                                 bool throwIfNotFound = true)
            where T : Enum
        {
            var attribute = value.GetFieldInfo().GetMetadata<MetaStringsAttribute>(throwIfNotFound: false);

            if (attribute.IsNullOrEmpty())
            {
                if (throwIfNotFound)
                    throw new MetadataAttributeNotFoundException();
                else
                    return Enum.GetNames(typeof(T));
            }

            return attribute.Single().Value;
        }
    }
}