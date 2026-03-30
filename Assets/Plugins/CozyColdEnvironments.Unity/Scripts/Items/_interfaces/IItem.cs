using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IItem : IEquatable<IItem>, IIDMarked<int>
    {
        string Name { get; }

        Sprite Icon { get; }

        int MaxItemCount { get; }
    }
}
