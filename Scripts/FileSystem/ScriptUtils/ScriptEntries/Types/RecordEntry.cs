#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public record RecordEntry : TypeEntry
    {
        public RecordEntry() : base()
        {
            dataType = Syntax.DataType.Record;
        }

        public override string ToString() => base.ToString();
    }
    public record RecordEntry<TParent> : TypeEntry<TParent>
    {
        public RecordEntry() : base()
        {
            dataType = Syntax.DataType.Record;
        }

        public override string ToString() => base.ToString();
    }
}
