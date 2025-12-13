using CCEnvs.Snapshots;
using System;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public interface ISaveSystem
    {
        void Save(string path);

        void Load(string path);

        IDisposable Register(object obj);

        bool Unregister(object? obj);

        void RegisterType(Type type, Func<object, ISnapshot> serializableConverter);

        bool UnregisterType(Type type);

        bool IsTypeRegistered(Type? type);
    }
}
