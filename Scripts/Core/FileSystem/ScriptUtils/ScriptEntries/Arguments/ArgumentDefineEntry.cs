using System;
using UTIRLib.Attributes.Metadata;

#nullable enable

namespace UTIRLib.FileSystem.ScriptUtils
{
    public record ArgumentDefineEntry : ScriptEntry, ITypeProvider
    {
        public enum Modifier
        {
            [MetaString("")]
            None,

            [MetaString("ref")]
            Ref,

            [MetaString("out")]
            Out,

            [MetaString("in")]
            In,
        }

        [Flags]
        public enum OtherModifiers
        {
            [MetaString("")]
            None,

            [MetaString("this")]
            This,

            /// <summary>
            /// Sets type as nullable
            /// </summary>
            [MetaString("?")]
            MaybeNull,
        }

        public Modifier ModifierValue { get; set; }
        public OtherModifiers OtherModifiersValue { get; set; }
        public string ArgumentName { get; set; } = string.Empty;
        public object? DefaultValue { get; set; }
        public Type? DataType { get; set; }

        Type? ITypeProvider.TypeValue {
            get => DataType;
            set => DataType = value;
        }
        bool ITypeProvider.HasTypeValue => DataType != null;

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            WriteLine(Attributes);

            WriteWithWhitespace(ModifierValue);

            if (OtherModifiersValue.HasFlag(OtherModifiers.This))
                WriteWithWhitespace(OtherModifiers.This);

            if (OtherModifiersValue.HasFlag(OtherModifiers.MaybeNull))
            {
                Write(DataType);
                WriteWithWhitespace(OtherModifiers.MaybeNull);
            }
            else
                WriteWithWhitespace(DataType);

            if (DefaultValue is not null)
            {
                WriteWithWhitespace(ArgumentName);
                WriteWithWhitespace('=');
                Write(DefaultValue);
            }
            else
                Write(ArgumentName);
        }
    }

    public record ArgumentDefineEntry<TData> : ArgumentDefineEntry
    {
        public ArgumentDefineEntry() : base()
        {
            DataType = typeof(TData);
        }

        public override string ToString() => base.ToString();
    }
}