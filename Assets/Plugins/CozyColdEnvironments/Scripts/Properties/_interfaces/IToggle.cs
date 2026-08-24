using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Proeprties
{
    public interface IToggle
    {
        bool State { get; set; }

        bool Trigger();

        Observable<bool> ObserveState();
    }
}
