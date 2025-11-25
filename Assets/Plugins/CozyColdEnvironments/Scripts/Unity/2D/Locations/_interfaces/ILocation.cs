using CCEnvs.Unity._2D.Locations;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity._2D
{
    public interface ILocation
    {
        Result<ILocationLayer> this[string name] { get; }
        Result<ILocationLayer> this[Enum key] { get; }

        Result<ILocationLayer> GetLocationLayer<T>(T key) where T : unmanaged, Enum;

        BoundsInt GetCellBounds();

    }
}
