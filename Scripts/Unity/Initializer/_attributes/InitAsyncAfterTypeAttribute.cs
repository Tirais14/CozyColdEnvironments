using System;
using System.Linq;
using UTIRLib.Diagnostics;
using UTIRLib.Unity.TypeMatching;

namespace UTIRLib.Initables
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InitAsyncAfterTypeAttribute : InitAsyncAttribute
    {
        public Type[] InitableTypes { get; }

        /// <exception cref="CollectionArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public InitAsyncAfterTypeAttribute(params Type[] types)
        {
            if (types.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(types), types);
            if (types.Any(x => x.IsNot<IInitableAsync>()))
                throw new ArgumentException($"Has not {nameof(IInitableAsync)} object.");

            InitableTypes = types;
        }
    }
}
