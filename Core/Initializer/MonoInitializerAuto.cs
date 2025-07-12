#nullable enable

namespace UTIRLib.Initializer
{
    public abstract class MonoInitializerAuto : MonoInitializer
    {
        protected void Awake() => StartCoroutine(IntializeCoroutine());
    }
}