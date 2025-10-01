#nullable enable
using static CCEnvs.Syntax;

namespace CCEnvs.Files.ScriptUtils
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
