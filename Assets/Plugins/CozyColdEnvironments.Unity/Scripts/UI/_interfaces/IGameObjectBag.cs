using CCEnvs.FuncLanguage;
using R3;
using UnityEngine;
using System.Collections.Generic;
using System;
using ObservableCollections;

#nullable enable
#pragma warning disable S3444
namespace CCEnvs.Unity.UI
{
    public interface IGameObjectBag
        : 
        IObservableCollection<GameObject>
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

            ///// <summary>
            ///// Setup cell size of <see cref="UnityEngine.UI.LayoutGroup"/> reffering to <see cref="GameObject"/> size
            ///// </summary>
            //CellSizeOnLastAddedGameObject = 16,

            Default = ReparentByRootMarker 
                      | 
                      ActivateOnAdd
                      |
                      DeactivateOnRemove
                      //|
                      //CellSizeOnLastAddedGameObject
        }

        Settings settings { get; set; }
    }
}
