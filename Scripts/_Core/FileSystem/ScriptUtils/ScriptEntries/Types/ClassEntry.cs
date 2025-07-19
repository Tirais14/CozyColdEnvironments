#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public record ClassEntry : TypeEntry
    {
        public ClassEntry() : base()
        {
            dataType = Syntax.DataType.Class;
        }

        public override string ToString() => base.ToString();
    }
    public record ClassEntry<TParent> : TypeEntry<TParent>
    {
        public ClassEntry() : base()
        {
            dataType = Syntax.DataType.Class;
        }

        public override string ToString() => base.ToString();
    }
}
