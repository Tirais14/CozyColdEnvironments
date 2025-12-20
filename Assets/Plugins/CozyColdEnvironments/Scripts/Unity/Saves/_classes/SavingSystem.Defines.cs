using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public sealed partial class SavingSystem 
    {
        private readonly struct RegisteredObject : IEquatable<RegisteredObject>
        {
            private readonly IDictionary<Type, Func<object, ISnapshot>> converters;

            public object Object { get; }
            public Type ObjectType { get; }
            public SceneInfo? SceneInfo { get; }

            public RegisteredObject(
                object obj,
                SceneInfo? sceneInfo,
                IDictionary<Type, Func<object, ISnapshot>> converters)
            {
                Guard.IsNotNull(obj, nameof(obj));
                CC.Guard.IsNotNull(converters, nameof(converters));

                Object = obj;
                SceneInfo = sceneInfo;
                this.converters = converters;

                ObjectType = obj.GetType();
            }

            public static bool operator ==(RegisteredObject left, RegisteredObject right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(RegisteredObject left, RegisteredObject right)
            {
                return !(left == right);
            }

            public ISnapshot CreateSnapshot()
            {
                if (!converters.TryGetValue(ObjectType, out var converter))
                    throw new InvalidOperationException($"Registration of type \"{ObjectType}\" invalid");

                return converter(Object);
            }

            public override bool Equals(object? obj)
            {
                return obj is RegisteredObject @object && Equals(@object);
            }

            public bool Equals(RegisteredObject other)
            {
                return EqualityComparer<object>.Default.Equals(Object, other.Object)
                       &&
                       ObjectType.Equals(other.ObjectType)
                       &&
                       SceneInfo.Equals(other.SceneInfo)
                       &&
                       converters.Equals(other.converters);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Object, ObjectType, SceneInfo, converters);
            }

            public override string ToString()
            {
                if (this.IsDefault())
                    return StringHelper.EMPTY_OBJECT;

                return $"Object \"{Object}\"; scene info {SceneInfo};";
            }
        }

        private interface IKeyFactory
        {
            Maybe<string> CreateKey(); 
        }

        private sealed class KeyFactory<TObject> : IKeyFactory
        {
            private readonly TObject target;
            private readonly Func<TObject, string> keyFactory;

            public KeyFactory(TObject target, Func<TObject, string> keyFactory)
            {
                CC.Guard.IsNotNullTarget(target);
                Guard.IsNotNull(keyFactory, nameof(keyFactory));

                this.target = target;
                this.keyFactory = keyFactory;
            }

            public Maybe<string> CreateKey()
            {
                try
                {
                    return keyFactory(target);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }

                return null;
            }
        }

        private sealed class KeyFactory<TObject, TState> : IKeyFactory
        {
            private readonly TObject target;
            private readonly TState state;
            private readonly Func<TObject, TState, string> keyFactory;

            public KeyFactory(TObject target, TState state, Func<TObject, TState, string> keyFactory)
            {
                CC.Guard.IsNotNullTarget(target);
                CC.Guard.IsNotNull(state, nameof(state));
                Guard.IsNotNull(keyFactory, nameof(keyFactory));

                this.target = target;
                this.state = state;
                this.keyFactory = keyFactory;
            }

            public Maybe<string> CreateKey()
            {
                try
                {
                    return keyFactory(target, state);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }

                return null;
            }
        }

        private sealed class Registration : IDisposable
        {
            private readonly SavingSystem savingSystem;
            private readonly RegisteredObject regObj;

            public Registration(SavingSystem savingSystem, RegisteredObject regObj)
            {
                this.savingSystem = savingSystem;
                this.regObj = regObj;
            }

            private bool disposed;

            public void Dispose()
            {
                if (disposed)
                    return;

                savingSystem.UnregisterObjectInternal(regObj);

                if (savingSystem.sceneDisposables.TryGetValue(regObj.SceneInfo, out var disposables))
                    disposables.Remove(this);

                disposed = true;
            }
        }

        private readonly struct LoadedSnapshotKey : IEquatable<LoadedSnapshotKey>
        {
            public string Key { get; }
            public Type TargetType { get; }
            public SceneInfo? SceneInfo { get; }

            public LoadedSnapshotKey(string key, Type targetType, SceneInfo? sceneInfo)
            {
                Guard.IsNotNullOrWhiteSpace(key, nameof(key));
                Guard.IsNotNull(targetType, nameof(targetType));

                Key = key;
                TargetType = targetType;
                SceneInfo = sceneInfo;
            }

            public static bool operator ==(LoadedSnapshotKey left, LoadedSnapshotKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(LoadedSnapshotKey left, LoadedSnapshotKey right)
            {
                return !(left == right);
            }

            public override bool Equals(object? obj)
            {
                return obj is LoadedSnapshotKey key && Equals(key);
            }

            public bool Equals(LoadedSnapshotKey other)
            {
                return Key == other.Key
                       &&
                       TargetType == other.TargetType
                       &&
                       SceneInfo == other.SceneInfo;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key, TargetType, SceneInfo);
            }
        }
    }
}
