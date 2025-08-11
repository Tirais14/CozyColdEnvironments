#nullable enable

using System;
using System.Linq;
using UTIRLib.Attributes.Metadata;
using static UTIRLib.FileSystem.ScriptUtils.Syntax;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public record EnumEntry : ScriptEntry, IType
    {
        public AccessModifier AccessModifier { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public EnumType ParentType { get; set; } = EnumType.Int;
        public EnumFieldEntry[] Fields { get; set; } = Array.Empty<EnumFieldEntry>();

        OtherModifiers IType.OtherModifiers {
            get => OtherModifiers.None;
            set => _ = value;
        }
        DataType IType.DataType => DataType.Enum;
        Type[] IType.ParentTypes {
            get => new Type[] { ParentType.GetMetaType() };
            set => _ = value;
        }
        IScriptContent[] IType.Members {
            get => Fields;
            set => Fields = value.OfType<EnumFieldEntry>().ToArray();
        }
        IScriptContent[] IContentProvider.Content {
            get => Fields;
            set => Fields = value.OfType<EnumFieldEntry>().ToArray();
        }
        bool IContentProvider.HasContent => Fields.IsNotNullOrEmpty();

        public EnumEntry() : base()
        {
            TabulationsCount = 1;
        }

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            WriteLine(Attributes);

            WriteWithWhitespace(AccessModifier);
            WriteWithWhitespace(DataType.Enum);

            if (ParentType != EnumType.Int)
            {
                WriteWithWhitespace(TypeName);
                WriteWithWhitespace(':');
                WriteLine(ParentType);
            }
            else
                WriteLine(TypeName);

            WriteLine('{');

            WriteLine(Fields, tabulationsCount: 0);

            Write('}');
        }
    }
}