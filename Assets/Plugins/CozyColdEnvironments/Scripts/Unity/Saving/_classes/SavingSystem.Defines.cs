using CCEnvs.FuncLanguage;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saving
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

            public void Deconstruct(out object obj, out Type objType, out SceneInfo? sceneInfo)
            {
                obj = Object;
                objType = ObjectType;
                sceneInfo = SceneInfo;
            }

            public static bool operator ==(RegisteredObject left, RegisteredObject right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(RegisteredObject left, RegisteredObject right)
            {
                return !(left == right);
            }

            public ISnapshot ConvertToSnapshot()
            {
                if (!converters.TryGetValue(ObjectType, out var converter))
                    throw new InvalidOperationException($"Registration of type '{ObjectType}' invalid");

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
        }

        private interface IKeyFactory
        {
            Maybe<string> Invoke(); 
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

            public Maybe<string> Invoke()
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

            public Maybe<string> Invoke()
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
    }
}
