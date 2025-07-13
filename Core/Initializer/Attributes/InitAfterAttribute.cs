using System;

#nullable enable
namespace UTIRLib.Init
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InitAfterAttribute : InitAttribute
    {
        public Type Type { get; }

        /// <exception cref="ArgumentException"></exception>
        public InitAfterAttribute(Type type)
        {
            if (type.IsNot<IInitable>())
                throw new ArgumentException($"{type.GetProccessedName()} is not {nameof(IInitable)} type.");

            Type = type;
        }
    }
}
