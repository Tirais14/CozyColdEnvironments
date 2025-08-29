using System;
using UnityEngine;

#nullable enable

namespace UTIRLib.Diagnostics
{
    public class ComponentNotFoundException : TirLibException
    {
        public ComponentNotFoundException()
        {
        }

        public ComponentNotFoundException(Type componentType)
            : base($"Component {componentType.Name} not found.")
        {
        }

        public ComponentNotFoundException(Type componentType, GameObject context)
            : base($"Component {componentType.Name} not found in game object: {context.name}.")
        {
        }
    }
}