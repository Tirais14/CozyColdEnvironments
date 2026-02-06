using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Profiles
{
    public interface IUserProfile : IDisposable
    {
        Identifier ID { get; }
        Sprite? Icon { get; set; }
        string Name { get; }

        Observable<Sprite?> ObserveIcon();
    }
}
