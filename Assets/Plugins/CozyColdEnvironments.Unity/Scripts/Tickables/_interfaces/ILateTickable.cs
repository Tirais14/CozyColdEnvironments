using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Tickables
{
    public interface ILateTickable
    {
        void LateTick();
    }
}
