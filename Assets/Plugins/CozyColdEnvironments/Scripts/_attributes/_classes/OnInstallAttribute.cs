using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Attributes
{
    [AttributeUsage(
        AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true
        )]
    public class OnInstallAttribute : Attribute, ICCAttribute
    {
    
    }
}
