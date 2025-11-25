using CCEnvs.Diagnostics;
using UnityEngine;

#nullable enable
namespace CCEnvs.U2D
{
    public class LocationOutOfBoundsException : CCException
    {
        public LocationOutOfBoundsException() : base() 
        { 
        }

        public LocationOutOfBoundsException(Vector3 position)
            : 
            base(Sentence.Empty.Add($"{nameof(position)}: {position}.").ToString())
        {
        }
    }
}
