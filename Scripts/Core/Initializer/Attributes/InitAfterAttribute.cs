using System;

#nullable enable
namespace UTIRLib.Init
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InitAfterAttribute : InitAttribute
    {
        public Type[] ObjectTypes { get; }

        /// <exception cref="ArgumentException"></exception>
        public InitAfterAttribute(params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsNot<IInitable>())
                    throw new CollectionItemException($"Is not {nameof(IInitable)}.", i);
            }

            ObjectTypes = types;
        }
    }
}
