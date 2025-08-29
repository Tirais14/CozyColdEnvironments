using CCEnvs.Diagnostics;

#nullable enable
namespace CCEnvs
{
    public class EndlessLoopException : TirLibException
    {
        public EndlessLoopException()
        {
        }

        public EndlessLoopException(ulong iterations,
                                    string message) 
            : base($"{message.TrimEnd('.')}. Iterarions = {iterations}.")
        {
        }

        public EndlessLoopException(ulong iterations) : this(iterations, "Endless loop")
        {
        }
    }
}
