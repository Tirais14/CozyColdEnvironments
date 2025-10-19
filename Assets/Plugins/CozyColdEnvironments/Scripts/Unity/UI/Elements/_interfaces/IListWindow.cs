using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    /// <summary>
    /// Like basic list, but adapdted for using with <see cref="Component"/>.
    /// <see cref="{T}"/> maybe interface or another abstraction, but it must be casted to <see cref="Component"/>
    /// </summary>
    public interface IListWindow
    {
        IEnumerable<GameObject> GameObjects { get; }
        IEnumerable<Component> Values { get; }
        bool DestroyGameObjectIfLastComponentRemoved { get; set; }
        Component this[int index, bool _] { get; }

        /// <summary>
        /// Remove only from list, doesn't destroy and reparent to root of hierarchy
        /// </summary>
        GameObject TakeGameObject(int index);
        /// <summary>
        /// Clears and after add all childrens
        /// </summary>
        void Rebuild();
    }
    /// <inheritdoc cref="IListWindow"/>
    public interface IListWindow<T> : IList<(GameObject go, IList<T> values)>, IListWindow
    {
        new IEnumerable<T> Values { get; }
        new T this[int index, bool _] { get; }

        IEnumerable<Component> IListWindow.Values => Values.Cast<Component>();
        Component IListWindow.this[int index, bool _] => this[index, _].As<Component>();
    }
}
