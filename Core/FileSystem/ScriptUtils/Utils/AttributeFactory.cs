using System;
using UTIRLib.Attributes.Metadata;
using UTIRLib.Extensions;

#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public static class AttributeFactory
    {
        public static AttributeEntry CreateFlags() => AttributeEntry.Create<FlagsAttribute>();

        /// <exception cref="ArgumentNullException"></exception>
        public static AttributeEntry CreateMetaString(
            string data,
            int tabulationsCount = 0)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (!data.IsWrappedByDoubleQuotes())
                data = data.WrapByDoubleQuotes();

            var attribute = AttributeEntry.Create<MetaStringAttribute>(new ArgumentEntry(data));
            attribute.TabulationsCount = tabulationsCount;

            return attribute;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static AttributeEntry CreateMetaStrings(
            int tabulationsCount,
            params string[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (data.HasNullElement())
                throw new ArgumentException($"{data} has null.");

            var attribute = AttributeEntry.Create<MetaStringAttribute>(new ArgumentEntry(data));
            attribute.TabulationsCount = tabulationsCount;

            return attribute;
        }
        public static AttributeEntry CreateMetaStrings(params string[] data)
        {
            return CreateMetaStrings(tabulationsCount: 0, data);
        }
    }
}
