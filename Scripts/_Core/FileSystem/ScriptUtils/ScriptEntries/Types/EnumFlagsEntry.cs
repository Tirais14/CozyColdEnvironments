#nullable enable

namespace UTIRLib.FileSystem.ScriptUtils
{
    public sealed record EnumFlagsEntry : EnumEntry
    {
        public EnumFlagsEntry() : base()
        {
            Attributes = new AttributeEntry[] { AttributeFactory.CreateFlags() };
        }

        public override string ToString() => base.ToString();
    }
}