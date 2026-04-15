using System;
using System.Collections.Generic;
using CCEnvs.Unity._2D.Locations;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity._2D
{
    public interface ILocation : IEnumerable<ILocationLayer>
    {
        ILocationLayer this[string name] { get; }
        ILocationLayer this[Enum key] { get; }

        BoundsInt CellBounds { get; }

        ILocationLayer GetLocationLayer<T>(T key) where T : unmanaged, Enum;

    }
}
