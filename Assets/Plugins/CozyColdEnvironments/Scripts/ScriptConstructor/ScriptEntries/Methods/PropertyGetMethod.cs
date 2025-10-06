using System;
using static CCEnvs.Syntax;

#nullable enable
namespace CCEnvs.Files.ScriptUtils
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
