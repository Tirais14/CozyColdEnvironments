using System;
using System.Linq;
using UTIRLib.Attributes.Metadata;

#nullable enable
namespace UTIRLib.Attributes.Metadata
{
    public static class MetadataAttributeQueries
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static MetadataAttribute Single(this MetadataAttribute[] values, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return SingleInternal(values, type, throwIfNotFound: true);
        }
        public static T Single<T>(this MetadataAttribute[] values)
            where T : MetadataAttribute
        {
            return (T)SingleInternal(values, typeof(T), throwIfNotFound: true);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static MetadataAttribute? SingleOrDefault(this MetadataAttribute[] values,
                                                         Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return SingleInternal(values, type, throwIfNotFound: false);
        }
        public static T? SingleOrDefault<T>(this MetadataAttribute[] values)
            where T : MetadataAttribute
        {
            return (T?)SingleInternal(values, typeof(T), throwIfNotFound: false);
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="MetadataAttributeNotFoundException"></exception>
        public static MetadataAttribute[] Where(this MetadataAttribute[] values,
                                                Type type,
                                                bool throwIfNotFound = true)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            MetadataAttribute[] results = values.Where(x => x.GetType() == type).ToArray();

            if (results.IsNullOrEmpty())
            {
                if (throwIfNotFound)
                    throw new MetadataAttributeNotFoundException(type);
                else
                    return Array.Empty<MetadataAttribute>();
            }

            return results;
        }
        public static MetadataAttribute[] Where<T>(this MetadataAttribute[] values,
                                                   bool throwIfNotFound = true)
            where T : MetadataAttribute
        {
            return values.Where(typeof(T), throwIfNotFound).Cast<T>().ToArray();
        }

        private static MetadataAttribute SingleInternal(MetadataAttribute[] values,
                                                        Type type,
                                                        bool throwIfNotFound)
        {
            MetadataAttribute found = values.SingleOrDefault(x => x.GetType() == type);

            if (found is null && throwIfNotFound)
                throw new MetadataAttributeNotFoundException(type);

            return found!;
        }
    }
}
