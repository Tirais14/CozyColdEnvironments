#nullable enable
using static CozyColdEnvironments.FileSystem.ScriptUtils.Syntax;

namespace CozyColdEnvironments.FileSystem.ScriptUtils
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
