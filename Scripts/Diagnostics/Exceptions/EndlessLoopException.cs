using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib
{
    public class EndlessLoopException : TirLibException
    {
        public EndlessLoopException()
        {
        }

        public EndlessLoopException(long iterations,
                                    string message) 
            : base($"{message.TrimEnd('.')}. Iterarions = {iterations}.")
        {
        }

        public EndlessLoopException(long iterations) : this(iterations, "Endless loop")
        {
        }
    }
}
