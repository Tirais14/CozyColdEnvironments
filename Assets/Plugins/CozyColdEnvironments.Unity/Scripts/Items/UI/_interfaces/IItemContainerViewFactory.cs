using CCEnvs.Patterns.Factories;
using CCEnvs.UnityX.Items;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Items.UI
{
    public interface IItemContainerViewFactory
        :
        IFactory<int, GameObject, InstantiateParameters, CancellationToken, ValueTask<IReadOnlyList<GameObject>>>
    {
    }
}
