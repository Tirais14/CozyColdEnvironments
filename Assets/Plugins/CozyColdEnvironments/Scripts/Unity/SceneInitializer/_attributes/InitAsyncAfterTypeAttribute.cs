using CCEnvs.Diagnostics;
using System;

namespace CCEnvs.Unity.Initables
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InitAsyncAfterTypeAttribute : InitAsyncAttribute
    {
        public Type[] InitableTypes { get; }

        /// <exception cref="EmptyCollectionArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public InitAsyncAfterTypeAttribute(params Type[] types)
        {
            CC.Guard.CollectionArgument(types, nameof(types));

            InitableTypes = types;
        }
    }
}
