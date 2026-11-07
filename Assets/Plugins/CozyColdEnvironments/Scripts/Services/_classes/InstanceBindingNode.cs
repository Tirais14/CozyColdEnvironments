#nullable enable
using CCEnvs.FuncLanguage;
using System;
using System.Threading;

namespace CCEnvs.Services
{
    public class InstanceBindingNode : IBindingNode
    {
        private readonly object instance;

        public InstanceBindingNode(object instance)
        {
            CC.Guard.IsNotNull(instance, nameof(instance));

            this.instance = instance;
        }

        public object GetInstance() => instance;

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            if (instance is IUtilizable utilizable)
                utilizable.Utilize();
            else if (instance is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
