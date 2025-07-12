using System;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

#nullable enable

namespace UTIRLib.UI
{
    public interface IMenu
    {
        ReadOnlyArray<Type> SupportedTypes { get; }
        ReadOnlyArray<IMenuItem> MenuItems { get; }
        ReadOnlyArray<IMenuItem> TargetMenuItems { get; }

        void Open(Vector2 position, object target);

        bool TryOpen(Vector2 position, object target);

        void Close();

        bool IsSupportedObject(object? obj);
    }
}