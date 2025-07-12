using System;
using System.Linq;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.FileSystem.ScriptUtils
{
    public sealed record AttributeEntry : ScriptEntry, ITypeProvider, IContentProvider
    {
        public Type? AttributeType { get; set; }
        public ArgumentEntry[] AttributeArgs { get; set; } = Array.Empty<ArgumentEntry>();

        Type? ITypeProvider.TypeValue {
            get => AttributeType;
            set => AttributeType = value;
        }
        bool ITypeProvider.HasTypeValue => AttributeType != null;

        IScriptContent[] IContentProvider.Content {
            get => AttributeArgs ?? Array.Empty<IScriptContent>();
            set => AttributeArgs = value.OfType<ArgumentEntry>().ToArray();
        }
        bool IContentProvider.HasContent => AttributeArgs.IsNotNullOrEmpty();

        private AttributeEntry(params ArgumentEntry[] attributeArgs) : base()
        {
            AttributeArgs = attributeArgs;
        }

        public static AttributeEntry Create<T>(int tabulationsCount,
                                               params ArgumentEntry[] attributeArgs)
            where T : Attribute
        {
            return new AttributeEntry(attributeArgs) {
                AttributeType = typeof(T)
            };
        }
        public static AttributeEntry Create<T>(params ArgumentEntry[] attributeArgs)
            where T : Attribute
        {
            return Create<T>(tabulationsCount: 0, attributeArgs);
        }

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            Write('[');

            Write(AttributeType.GetProccessedName().Delete(nameof(Attribute)));

            string attributeArgsValue = AttributeArgsToString();
            if (attributeArgsValue.IsNotNullOrEmpty())
            {
                Write('(');

                Write(attributeArgsValue, tabulationsCount: 0);

                Write(')');
            }

            Write(']');
        }

        private string AttributeArgsToString()
        {
            string converted = ProccessValue(AttributeArgs);

            string[] convertedLines = converted.SplitByLines(StringSplitOptions.RemoveEmptyEntries);

            return convertedLines.JoinStrings(", ");
        }
    }
}