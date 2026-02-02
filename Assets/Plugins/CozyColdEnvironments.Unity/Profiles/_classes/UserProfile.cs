using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Profiles
{
    public sealed class UserProfile : IUserProfile, IEquatable<UserProfile>
    {
        public static UserProfile Empty => new("undefined", "undefined");

        private readonly ReactiveProperty<Sprite?> profileIcon = new();

        public Identifier ID { get; }

        public Sprite? Icon {
            get => profileIcon.Value;
            set => profileIcon.Value = value;
        }

        public string Name { get; }

        public UserProfile(Identifier? id, string name)
        {
            ID = id ?? Guid.NewGuid().ToString();
            Name = name ?? string.Empty;
        }

        public static bool operator ==(UserProfile left, UserProfile right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UserProfile left, UserProfile right)
        {
            return !(left == right);
        }

        public bool Equals(UserProfile other)
        {
            return Icon == other.Icon
                   &&
                   Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            return obj is UserProfile info && Equals(info);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Icon, Name);
        }

        public override string ToString()
        {
            return $"({nameof(ID)}: {ID}; {nameof(Name)}: {nameof(Name)})";
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
        }

        public Observable<Sprite?> ObserveIcon()
        {
            return profileIcon;
        }
    }
}
