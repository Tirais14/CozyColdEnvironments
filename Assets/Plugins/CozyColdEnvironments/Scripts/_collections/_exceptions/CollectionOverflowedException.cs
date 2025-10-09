using CCEnvs.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Collections
{
    public class CollectionOverflowedException : CCException
    {
        public CollectionOverflowedException(Exception? innerException = null)
            :
            base("Limited collection is full.", innerException)
        {
        }

        public CollectionOverflowedException(string message, Exception? innerException = null) 
            :
            base($"Limited collection is full. {message}", innerException)
        {
        }
    }
}
