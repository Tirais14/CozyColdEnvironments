using System;
using static UTIRLib.FileSystem.ScriptUtils.Syntax;

#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
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
