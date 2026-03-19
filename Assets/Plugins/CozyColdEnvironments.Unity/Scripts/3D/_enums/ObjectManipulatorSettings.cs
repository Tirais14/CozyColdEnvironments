using System;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [Flags]
    public enum ObjectManipulatorSettings
    {
        None,
        CollideWithSurface,
        ObjectSizeChangeable,
        Default = CollideWithSurface
    }
}
