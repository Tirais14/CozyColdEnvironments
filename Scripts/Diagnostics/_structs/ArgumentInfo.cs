#nullable enable

using System;
namespace CozyColdEnvironments.Diagnostics.Internal
{
    public readonly struct ArgumentInfo
    {
        public readonly object? value;
        public readonly Type valueType;

        public ArgumentInfo(object? value, Type valueType)
        {
            this.value = value;
            this.valueType = valueType;
        }
    }
}