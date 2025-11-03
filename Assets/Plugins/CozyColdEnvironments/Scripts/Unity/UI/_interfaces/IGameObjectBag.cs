using CCEnvs.FuncLanguage;
using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System;

#nullable enable
#pragma warning disable S3444
namespace CCEnvs.Unity.UI.Elements
{
    public interface IGameObjectBag
        : IReadOnlyReactiveCollection<GameObject>,
        ICollection<GameObject>
    {
        [Flags]
        public enum Settings
        {
            None,
            DestroyOnRemove = 1,
            /// <summary>
            /// Use <see cref="RootMarker"/>.transform otherwise use added <see cref="GameObject.transform"/>
            /// </summary>
            ReparentByRootMarker = 2,
            ActivateOnAdd = 4,
            DeactivateOnRemove = 8,
            Default = ReparentByRootMarker | ActivateOnAdd | DeactivateOnRemove
        }

        Settings settings { get; set; }

        int IReadOnlyReactiveCollection<GameObject>.Count => this.As<IReadOnlyCollection<GameObject>>().Count;
        bool ICollection<GameObject>.IsReadOnly => false;
    }
}
