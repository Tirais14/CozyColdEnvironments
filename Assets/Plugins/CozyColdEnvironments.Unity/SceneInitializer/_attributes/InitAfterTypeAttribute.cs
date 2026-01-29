using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Linq;

#nullable enable
namespace CCEnvs.Unity.Initables
{
    /// <summary>
    /// Call <see cref="IInitable.Init"/> only after initialization this types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InitAfterTypeAttribute : InitAttribute
    {
        public Type[] ObjectTypes { get; }

        /// <exception cref="ArgumentException"></exception>
        public InitAfterTypeAttribute(params Type[] types)
        {
            Guard.IsNotNull(types, nameof(types));

            if (types.Any(x => x.IsNotType<IInitable>()))
                throw new ArgumentException("Invalid type in collection.");

            ObjectTypes = types;
        }
    }
}
