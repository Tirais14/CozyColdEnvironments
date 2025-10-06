#nullable enable
namespace CCEnvs.Files.ScriptUtils
{
    public record StructEntry : TypeEntry
    {
        public StructEntry() : base()
        {
            dataType = Syntax.DataType.Struct;
        }

        public override string ToString() => base.ToString();
    }
}
