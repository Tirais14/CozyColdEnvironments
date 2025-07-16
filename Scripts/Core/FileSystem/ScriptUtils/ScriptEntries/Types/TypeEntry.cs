#nullable enable
using System;
using System.Text;
using static UTIRLib.FileSystem.ScriptUtils.Syntax;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public abstract record TypeEntry : ScriptEntry, IType
    {
        protected DataType dataType;

        public AccessModifier AccessModifier { get; set; }
        public OtherModifiers OtherModifiers { get; set; }
        public DataType DataType => dataType;
        public string TypeName { get; set; } = string.Empty;
        public virtual Type[] ParentTypes { get; set; } = Array.Empty<Type>();
        public IScriptContent[] Members { get; set; } = Array.Empty<IScriptContent>();

        IScriptContent[] IContentProvider.Content {
            get => Members ?? Array.Empty<IScriptContent>();
            set => Members = value;
        }
        bool IContentProvider.HasContent => Members.IsNotNullOrEmpty();

        protected TypeEntry() : base()
        {
            TabulationsCount = 1;
        }

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            WriteLine(Attributes);

            WriteWithWhitespace(AccessModifier);
            WriteWithWhitespace(OtherModifiers);
            WriteWithWhitespace(dataType);

            if (TryGetParentTypesString(out string parentTypesString))
            {
                WriteWithWhitespace(TypeName);
                WriteWithWhitespace(':');
                WriteLine(parentTypesString);
            }
            else
                WriteLine(TypeName);

            WriteLine('{');

            WriteLine(Members, tabulationsCount: 0);

            Write('}');
        }

        private bool TryGetParentTypesString(out string result)
        {
            if (ParentTypes.IsNullOrEmpty())
            {
                result = string.Empty;
                return false;
            }

            var resultBuilder = new StringBuilder();
            for (int i = 0; i < ParentTypes.Length; i++)
            {
                if (i != 0)
                    resultBuilder.Append(", ");

                resultBuilder.Append(ParentTypes[i].GetProccessedName());
            }

            result = resultBuilder.ToString();
            return true;
        }
    }

    public record TypeEntry<TParent> : TypeEntry
    {
        public TypeEntry()
        {
            ParentTypes = new Type[] { typeof(TParent) };
        }

        public override string ToString() => base.ToString();
    }
}