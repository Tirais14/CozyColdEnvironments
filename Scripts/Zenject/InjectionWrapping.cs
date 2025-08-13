#nullable enable
namespace UTIRLib.Zenject
{
    public abstract class InjectionWrapping<T>
    {
        public readonly T Value;

        protected InjectionWrapping(T value)
        {
            Value = value;
        }

        public static implicit operator T(InjectionWrapping<T> wrapper)
        {
            return wrapper.Value;
        }
    }
}
