#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public record InterfaceEntry : TypeEntry
    {
        public InterfaceEntry() : base()
        {
            AccessModifier = Syntax.AccessModifier.None;
            dataType = Syntax.DataType.Interface;
        }

        public override string ToString() => base.ToString();
    }

    public record InterfaceEntry<TParent> : TypeEntry<TParent>
    {
        public InterfaceEntry() : base()
        {
            AccessModifier = Syntax.AccessModifier.None;
            dataType = Syntax.DataType.Interface;
        }

        public override string ToString() => base.ToString();
    }
}
