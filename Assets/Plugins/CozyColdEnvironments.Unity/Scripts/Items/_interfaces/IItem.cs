using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public interface IItem : IIDMarked<int>
    {
        string Name { get; }

        Sprite Icon { get; }

        int MaxItemCount { get; }
    }
}
