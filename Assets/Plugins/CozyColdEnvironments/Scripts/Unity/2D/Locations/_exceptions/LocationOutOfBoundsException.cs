using CCEnvs.Diagnostics;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.U2D
{
    public class LocationOutOfBoundsException : CCException
    {
        public LocationOutOfBoundsException()
        {
        }

        public LocationOutOfBoundsException(string message, Exception? innerException = null)
            : 
            base(message, innerException)
        {
        }

        public LocationOutOfBoundsException(Vector2 pos,
                                            Exception? innerException = null) 
            :
            base($"Position: {pos}.", innerException)
        {
        }
    }
}
