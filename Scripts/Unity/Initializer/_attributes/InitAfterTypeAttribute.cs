using System;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.Initables
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
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsNotType<IInitable>())
                    throw new CollectionItemException($"{types[i].GetName()} is not {nameof(IInitable)}.", i);
            }

            ObjectTypes = types;
        }
    }
}
