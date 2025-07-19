#nullable enable
using static UTIRLib.FileSystem.ScriptUtils.Syntax;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public record PropertyEntry : ScriptEntry
    {
        public AccessModifier AccessModifier { get; set; }

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {

        }
    }
}
