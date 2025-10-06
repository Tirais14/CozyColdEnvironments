using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IButton
    {
        Action OnClick { get; }
    }
}
