using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Collections
{
    public static class TEnumerable<T>
    {
        public static IEnumerable<T> Empty { get; } = new List<T>();
    }
}
