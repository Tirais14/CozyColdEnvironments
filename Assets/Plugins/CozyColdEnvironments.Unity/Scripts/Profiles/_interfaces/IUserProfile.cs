using System;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Profiles
{
    public interface IUserProfile : IDisposable
    {
        Identifier ID { get; }
        Sprite? Icon { get; set; }
        string Name { get; }

        Observable<Sprite?> ObserveIcon();
    }
}
