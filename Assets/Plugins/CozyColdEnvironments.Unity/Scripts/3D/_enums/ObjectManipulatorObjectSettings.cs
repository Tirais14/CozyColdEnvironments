using System;

#nullable enable
namespace CCEnvs.UnityX.D3
{
    [Flags]
    public enum ObjectManipulatorObjectSettings
    {
        None,
        CollideWithSurface,
        ObjectSizeChangeable,
        SlideOnSurface,
        Default = CollideWithSurface
    }
}
