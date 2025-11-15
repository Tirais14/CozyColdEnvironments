using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IItem : IEquatable<IItem>
    {
        string Name { get; }
        int ID { get; }
        Sprite Icon { get; }
        int MaxItemCount { get; }
    }
}
