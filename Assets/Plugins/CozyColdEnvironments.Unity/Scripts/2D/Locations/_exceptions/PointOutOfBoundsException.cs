using CCEnvs.Diagnostics;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX._2D
{
    public class PointOutOfBoundsException : CCException
    {
        public PointOutOfBoundsException() : base()
        {
        }

        public PointOutOfBoundsException(Vector3 position)
            :
            base(Sentence.Empty.Add($"{nameof(position)}: {position}.").ToString())
        {
        }
    }
}
