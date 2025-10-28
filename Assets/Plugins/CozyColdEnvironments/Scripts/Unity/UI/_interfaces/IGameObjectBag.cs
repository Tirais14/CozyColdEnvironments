using CCEnvs.FuncLanguage;
using UniRx;
using UnityEngine;
using System.Collections.Generic;

#nullable enable
#pragma warning disable S3444
namespace CCEnvs.Unity.UI.Elements
{
    public interface IGameObjectBag
        : IReadOnlyReactiveCollection<GameObject>,
        ICollection<GameObject>
    {
        bool DestroyOnRemove { get; set; }

        int IReadOnlyReactiveCollection<GameObject>.Count => this.As<IReadOnlyCollection<GameObject>>().Count;
        bool ICollection<GameObject>.IsReadOnly => false;
    }
}
