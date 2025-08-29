#nullable enable
using static CCEnvs.FileSystem.ScriptUtils.Syntax;

namespace CCEnvs.FileSystem.ScriptUtils
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
