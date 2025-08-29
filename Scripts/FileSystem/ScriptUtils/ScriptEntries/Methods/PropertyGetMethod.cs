using System;
using static CozyColdEnvironments.FileSystem.ScriptUtils.Syntax;

#nullable enable
namespace CozyColdEnvironments.FileSystem.ScriptUtils
{
    public record PropertyGetMethod : PropertyMethod
    {
        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            if (ByLambda)
            {
                Write("get => ");

                Write(BodyLines);
            }
            else
            {
                WriteLine("get {");

                WriteLine(BodyLines);

                Write('}');
            }
        }
    }
}
