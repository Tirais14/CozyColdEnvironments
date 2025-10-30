using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs.FuncLanguage
{
    public class ValueIsNoneException : CCException
    {
        public ValueIsNoneException() 
            :
            base($"Value {nameof(IConditional.IsNone)}.")
        {
        }
    }
}
