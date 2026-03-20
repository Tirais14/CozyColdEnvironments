using System;

#nullable enable
namespace CCEnvs.Unity.D3
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
