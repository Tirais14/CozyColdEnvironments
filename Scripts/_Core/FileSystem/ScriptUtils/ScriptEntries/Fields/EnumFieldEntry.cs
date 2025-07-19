#nullable enable

namespace UTIRLib.FileSystem.ScriptUtils
{
    public sealed record EnumFieldEntry : FieldEntry
    {
        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            WriteLine(Attributes);

            Write(FieldName);

            Write(FieldValue);

            Write(',');
        }
    }
}