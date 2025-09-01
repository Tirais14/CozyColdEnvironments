#nullable enable
using CCEnvs.Reflection;
using System;

namespace CCEnvs.Diagnostics
{
    public class InstanceCreationException : CCException
    {
        public InstanceCreationException()
        {
        }

        public InstanceCreationException(Type instanceType) 
            : 
            base($"Instance type = {instanceType.GetName()}.")
        {
        }
    }
}
