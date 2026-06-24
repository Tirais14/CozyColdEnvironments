using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public interface IItem : IIDMarked<int>, INamed
    {
        Sprite Icon { get; }

        int MaxItemCount { get; }
    }
}
