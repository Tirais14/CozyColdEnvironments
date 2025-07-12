using static UTIRLib.FileSystem.ScriptUtils.Syntax;

#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public record PropertySetMethod : PropertyMethod
    {
        public AccessModifier AccessModifier { get; set; }

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            if (ByLambda)
            {
                WriteWithWhitespace(AccessModifier);

                Write("set => ");

                Write(BodyLines);
            }
            else
            {
                WriteWithWhitespace(AccessModifier);

                WriteLine("set {");

                WriteLine(BodyLines);

                Write('}');
            }
        }
    }
}
