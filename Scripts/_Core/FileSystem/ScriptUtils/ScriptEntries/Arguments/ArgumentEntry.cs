#nullable enable
using System;
using UTIRLib.Attributes.Metadata;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public record ArgumentEntry : ScriptEntry, ITypeProvider
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
            In
        }

        public Modifier ModifierValue { get; set; }
        public object? ArgumentValue { get; set; }
        public Type? DataType => ArgumentValue?.GetType();

        public ArgumentEntry(object? argumentValue)
        {
            ArgumentValue = argumentValue;
        }

        Type? ITypeProvider.TypeValue {
            get => DataType;
            set => _ = value;
        }
        bool ITypeProvider.HasTypeValue => DataType != null;

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            WriteWithWhitespace(ModifierValue);

            Write(ArgumentValue);
        }
    }

    public record ArgumentEntry<T> : ArgumentEntry
    {
        new public T? ArgumentValue {
            get => (T?)base.ArgumentValue;
            set => base.ArgumentValue = value;
        }

        public ArgumentEntry(T? argumentValue) : base(argumentValue)
        {
        }

        public override string ToString() => base.ToString();
    }
}
