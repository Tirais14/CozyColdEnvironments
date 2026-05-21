using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Collections
{
    public interface IComponentEnumerable : IUnityObjectEnumerable, IEnumerable<Component>
    {
        IEnumerator<Object> IEnumerable<Object>.GetEnumerator()
        {
            return this.Cast<Object>().GetEnumerator();
        }
    }
}
