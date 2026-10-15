#nullable enable
namespace CCEnvs.Services
{
    public class ServiceBinderGeneric<T>
    {
        public ServiceBinderGeneric() { }

        public ServiceInstanceBinderGeneric<T> FromInstance(T instance)
        {
            return new ServiceInstanceBinderGeneric<T>(instance);
        }
    }
}
