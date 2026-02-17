using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Profiles
{
    public sealed class UserProfile : IUserProfile
    {
        public static UserProfile Empty => new("undefined", Guid.NewGuid().ToString());

        private readonly ReactiveProperty<Sprite?> profileIcon = new();

        public Identifier ID { get; }

        public Sprite? Icon {
            get => profileIcon.Value;
            set => profileIcon.Value = value;
        }

        public string Name { get; }

        public UserProfile(string name, Identifier? id = null)
        {
            Name = name ?? string.Empty;

            ID = new Identifier(id?.Number, $"{name}:{id?.Text ?? Guid.NewGuid().ToString()}");
        }

        public override string ToString()
        {
            return $"({nameof(Name)}: {Name}; {nameof(ID)}: {ID})";
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
