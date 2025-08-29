using System;

#nullable enable
namespace CozyColdEnvironments.Initables
{
    /// <summary>
    /// Marks type to init in first order. Uses as entry point to initializing scene by attributes. Maybe any amount on the scene.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InitFirstAttribute : InitAttribute
    {
    }
}
