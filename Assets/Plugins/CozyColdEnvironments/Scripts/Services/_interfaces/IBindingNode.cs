using System;

#nullable enable
namespace CCEnvs.Services
{
    public interface IBindingNode : IDisposable
    {
        public object GetInstance();
    }   
}
