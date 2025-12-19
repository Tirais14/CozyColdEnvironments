using CommunityToolkit.Diagnostics;
using System;

namespace CCEnvs.Unity.Initables
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InitAsyncAfterTypeAttribute : InitAsyncAttribute
    {
        public Type[] InitableTypes { get; }

        /// <exception cref="ArgumentException"></exception>
        public InitAsyncAfterTypeAttribute(params Type[] types)
        {
            Guard.IsNotNull(types, nameof(types));

            InitableTypes = types;
        }
    }
}
