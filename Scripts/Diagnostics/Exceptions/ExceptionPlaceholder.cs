using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionPlaceholder : TirLibException
    {
        public ExceptionPlaceholder()
        {
        }

        public ExceptionPlaceholder(string message) : base(message)
        {
        }

        public ExceptionPlaceholder(string notFormattedMessage,
                                    params object[] args) : base(notFormattedMessage,
                                                                 args)
        {
        }
    }
}
