using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity
{
    public readonly struct SceneBindedObject<T> : IEquatable<SceneBindedObject<T>>
    {
        public T Value { get; }
        public SceneInfo SceneInfo { get; }

        [JsonConstructor]
        public SceneBindedObject(T value, SceneInfo sceneInfo)
        {
            Value = value;
            SceneInfo = sceneInfo;
        }

        public static bool operator ==(SceneBindedObject<T> left, SceneBindedObject<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SceneBindedObject<T> left, SceneBindedObject<T> right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is SceneBindedObject<T> @object && Equals(@object);
        }

        public bool Equals(SceneBindedObject<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value)
                   &&
                   SceneInfo.Equals(other.SceneInfo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, SceneInfo);
        }
    }
}
