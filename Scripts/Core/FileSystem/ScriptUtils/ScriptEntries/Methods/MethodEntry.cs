using System;
using UTIRLib.Attributes.Metadata;
using static UTIRLib.FileSystem.ScriptUtils.Syntax;

#nullable enable

namespace UTIRLib.FileSystem.ScriptUtils
{
    public record MethodEntry : ScriptEntry,
        IMethod,
        ITypeMember,
        ITypeProvider,
        IArgumentsDefineProvider
    {
        [Flags]
        public enum Modifiers
        {
            [MetaString("")]
            None,

            [MetaString("extern")]
            Extern,

            [MetaString("partial")]
            Partial = 2,

            [MetaString("static")]
            Static = 4,

            [MetaString("readonly")]
            Readonly = 8,

            [MetaString("async")]
            Async = 16,
        }

        public AccessModifier AccessModifier { get; set; } = AccessModifier.Private;
        public Modifiers ModifierFlags { get; set; }
        public Type? ReturnType { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public ArgumentDefineEntry[] DefineArguments { get; set; } = Array.Empty<ArgumentDefineEntry>();
        public bool HasDefineArguments => DefineArguments.IsNotNullOrEmpty();
        public IScriptContent[] BodyLines { get; set; } = Array.Empty<IScriptContent>();
        public bool ByLambda { get; set; }

        IScriptContent[] IContentProvider.Content {
            get => BodyLines;
            set => BodyLines = value;
        }
        bool IContentProvider.HasContent => BodyLines.IsNotNullOrEmpty();

        Type? ITypeProvider.TypeValue {
            get => ReturnType;
            set => ReturnType = value;
        }
        bool ITypeProvider.HasTypeValue => ReturnType != null;

        public MethodEntry() : base()
        {
            TabulationsCount = 2;
        }

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            WriteWithWhitespace(Attributes);

            WriteWithWhitespace(AccessModifier);
            WriteWithWhitespace(ModifierFlags);

            WriteWithWhitespace(ReturnType?.GetProccessedName() ?? "void");

            Write('(');
            Write(DefineArguments);

            if (ByLambda)
            {
                WriteWithWhitespace(')');

                Write("=> ");

                Write(BodyLines);

                Write(';');
            }
            else
            {
                WriteLine(')');

                WriteLine('{');

                WriteLine(BodyLines);

                Write('}');
            }
        }
    }

    public record MethodEntry<TReturn> : MethodEntry
    {
        public MethodEntry() => ReturnType = typeof(TReturn);

        public override string ToString() => base.ToString();
    }
}