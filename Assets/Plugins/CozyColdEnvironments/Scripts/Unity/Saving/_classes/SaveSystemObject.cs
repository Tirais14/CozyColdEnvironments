using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public readonly struct SaveSystemObject : IEquatable<SaveSystemObject>
    {
        private readonly IDictionary<Type, Func<object, ISnapshot>> converters;

        public object Object { get; }
        public Type ObjectType { get; }
        public SceneInfo SceneInfo { get; }
        public string Key { get; }

        public SaveSystemObject(
            object obj,
            SceneInfo sceneInfo,
            string key,
            IDictionary<Type, Func<object, ISnapshot>> converters)
        {
            Guard.IsNotNull(obj, nameof(obj));
            Guard.IsNotNull(key, nameof(key));
            CC.Guard.IsNotNull(converters, nameof(converters)); 

            Object = obj;
            SceneInfo = sceneInfo;
            Key = key;
            this.converters = converters;

            ObjectType = obj.GetType();
        }

        public static bool operator ==(SaveSystemObject left, SaveSystemObject right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveSystemObject left, SaveSystemObject right)
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
            return obj is SaveSystemObject @object && Equals(@object);
        }

        public bool Equals(SaveSystemObject other)
        {
            return EqualityComparer<object>.Default.Equals(Object, other.Object)
                   &&
                   ObjectType.Equals(other.ObjectType)
                   &&
                   SceneInfo.Equals(other.SceneInfo)
                   &&
                   Key.Equals(other.Key)
                   &&
                   converters.Equals(other.converters);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Object, ObjectType, SceneInfo, Key, converters);
        }
    }
}
