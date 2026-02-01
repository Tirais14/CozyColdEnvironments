using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Profiles
{
    public interface IUserProfile : IDisposable
    {
        Identifier ID { get; }
        Sprite? Icon { get; }
        string Name { get; }

        Observable<Sprite?> ObserveIcon();

        Observable<string> ObserveName();
    }
}
