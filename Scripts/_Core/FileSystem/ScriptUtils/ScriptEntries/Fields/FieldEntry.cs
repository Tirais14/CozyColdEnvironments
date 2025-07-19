using System;
using UTIRLib.Attributes.Metadata;
using UTIRLib.Extensions;
using static UTIRLib.FileSystem.ScriptUtils.Syntax;

#nullable enable

namespace UTIRLib.FileSystem.ScriptUtils
{
    public record FieldEntry : ScriptEntry, IField
    {
        [Flags]
        public enum Modifiers
        {
            [MetaString("")]
            None,

            [MetaString("const")]
            Const,

            [MetaString("static")]
            Static = 2,

            [MetaString("readonly")]
            Readonly = 4
        }

        protected object? _fieldValue;
        protected Type? _dataType;

        public AccessModifier AccessModifier { get; set; } = AccessModifier.Private;
        public OtherModifiers OtherModifierFlags { get; set; }
        public Modifiers ModifiersFlags { get; set; }
        public string? FieldName { get; set; }
        public object? FieldValue { get; set; }
        public Type? DataType { get; set; }

        Type? ITypeProvider.TypeValue {
            get => DataType;
            set => DataType = value;
        }
        public bool HasTypeValue => DataType != null;

        public FieldEntry() : base()
        {
            TabulationsCount = 2;
        }

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            WriteLine(Attributes);

            WriteWithWhitespace(AccessModifier);

            WriteWithWhitespace(ResolveModifiers());

            WriteWithWhitespace(OtherModifierFlags);

            WriteWithWhitespace(DataType);

            WriteWithWhitespace(FieldName);

            Write(FieldValueToString());

            Write(';');
        }

        private string FieldValueToString()
        {
            string fieldValue;

            if (FieldValue is string strFieldValue)
            {
                if (!strFieldValue.StartsWith('\"') && !strFieldValue.EndsWith('\"'))
                    strFieldValue = strFieldValue.WrapByDoubleQuotes();

                fieldValue = strFieldValue;
            }
            else
                fieldValue = ProccessValue(FieldValue);

            if (fieldValue.IsNotNullOrEmpty())
                return "= " + fieldValue;

            return string.Empty;
        }

        private string ResolveModifiers()
        {
            if (ModifiersFlags.HasFlag(Modifiers.Const))
                return Modifiers.Const.GetMetaString();

            string result = string.Empty;
            if (ModifiersFlags.HasFlag(Modifiers.Readonly))
                result += Modifiers.Readonly.GetMetaString() + ' ';

            if (ModifiersFlags.HasFlag(Modifiers.Static))
                result += Modifiers.Static.GetMetaString() + ' ';

            return result;
        }
    }

    public record FieldEntry<T> : FieldEntry
    {
        new public T? FieldValue {
            get => (T?)base.FieldValue;
            set => base.FieldValue = value;
        }

        public FieldEntry() => DataType = typeof(T);

        public override string ToString() => base.ToString();
    }
}