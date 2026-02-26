using CCEnvs.Caching;
using Humanizer;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    public struct MethodNameBuilder : IEquatable<MethodNameBuilder>
    {
        internal static Cache<MethodNameBuilder, string> cachedStrings = new();

        private int? hash;

        private string? str;

        public string? Name { readonly get; init; }

        public Identifier ID { readonly get; init;  }

        public static MethodNameBuilder<TCaller> From<TCaller>(
            TCaller caller,
            string? name,
            Identifier id
            )
        {
            return new MethodNameBuilder<TCaller>
            {
                Caller  = caller,
                Name = name,
                ID = id
            };
        }

        public static implicit operator string(MethodNameBuilder instance)
        {
            return instance.ToString();
        }

        public static implicit operator MethodNameBuilder(string? name)
        {
            return new MethodNameBuilder
            {
                Name = name,
            };
        }

        public static implicit operator MethodNameBuilder(Identifier id)
        {
            return new MethodNameBuilder
            {
                ID = id,
            };
        }

        public static implicit operator MethodNameBuilder((string Name, Identifier ID) tuple)
        {
            return new MethodNameBuilder
            {
                Name = tuple.Name,
                ID = tuple.ID
            };
        }

        public static bool operator ==(MethodNameBuilder left, MethodNameBuilder right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MethodNameBuilder left, MethodNameBuilder right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodNameBuilder WithName(string? name)
        {
            return new MethodNameBuilder
            {
                Name = name,
                ID = ID
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodNameBuilder WithID(Identifier id)
        {
            return new MethodNameBuilder
            {
                Name = Name,
                ID = id
            };
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is MethodNameBuilder builder && Equals(builder);
        }

        public readonly bool Equals(MethodNameBuilder other)
        {
            return Name == other.Name
                   &&
                   ID.Equals(other.ID);
        }

        public override int GetHashCode()
        {
            hash ??= HashCode.Combine(Name, ID);

            return hash.Value;
        }

        public override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            if (str is null)
            {
                if (!cachedStrings.TryGet(this, out str))
                {
                    str ??= $"({nameof(Name)}: {Name}; {nameof(ID)}: {ID})";

                    if (cachedStrings.TryAdd(this, str, out var entry))
                        entry.ExpirationTimeRelativeToNow = 15.Minutes();
                }
            }

            return str;
        }
    }

    public struct MethodNameBuilder<TCaller> : IEquatable<MethodNameBuilder<TCaller>>
    {
        private int? hash;

        private string? str;

        public TCaller Caller { readonly get; init; }

        public string? Name { readonly get; init; }

        public Identifier ID { readonly get; init; }

        public static implicit operator string(MethodNameBuilder<TCaller> instance)
        {
            return instance.ToString();
        }

        public static implicit operator MethodNameBuilder(MethodNameBuilder<TCaller> instance)
        {
            return new MethodNameBuilder
            {
                Name = instance.Name,
                ID = instance.ID
            };
        }

        public static implicit operator MethodNameBuilder<TCaller>(TCaller caller)
        {
            return new MethodNameBuilder<TCaller>
            {
                Caller = caller,
            };
        }

        public static implicit operator MethodNameBuilder<TCaller>(string? name)
        {
            return new MethodNameBuilder<TCaller>
            {
                Name = name,
            };
        }

        public static implicit operator MethodNameBuilder<TCaller>(Identifier id)
        {
            return new MethodNameBuilder<TCaller>
            {
                ID = id,
            };
        }

        public static implicit operator MethodNameBuilder<TCaller>((TCaller Caller, string Name) tuple)
        {
            return new MethodNameBuilder<TCaller>
            {
                Caller = tuple.Caller,
                Name = tuple.Name,
            };
        }

        public static implicit operator MethodNameBuilder<TCaller>((TCaller Caller, string Name, Identifier ID) tuple)
        {
            return new MethodNameBuilder<TCaller>
            {
                Caller = tuple.Caller,
                Name = tuple.Name,
                ID = tuple.ID
            };
        }

        public static bool operator ==(MethodNameBuilder<TCaller> left, MethodNameBuilder<TCaller> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MethodNameBuilder<TCaller> left, MethodNameBuilder<TCaller> right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodNameBuilder<TCaller> WithName(string? name)
        {
            return new MethodNameBuilder<TCaller>
            {
                Caller = Caller,
                Name = name,
                ID = ID
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodNameBuilder<TCaller> WithID(Identifier id)
        {
            return new MethodNameBuilder<TCaller>
            {
                Caller = Caller,
                Name = Name,
                ID = id,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodNameBuilder<TCaller> WithCaller(TCaller caller)
        {
            return new MethodNameBuilder<TCaller>
            {
                Caller = caller,
                Name = Name,
                ID = ID
            };
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is MethodNameBuilder<TCaller> builder && Equals(builder);
        }

        public readonly bool Equals(MethodNameBuilder<TCaller> other)
        {
            return Name == other.Name
                   &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            hash ??= HashCode.Combine(Name, ID);

            return hash.Value;
        }

        public override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            if (str is null)
            {
                if (!MethodNameBuilder.cachedStrings.TryGet(this, out str))
                {
                    str = $"({(Caller.IsNotNull() ? Caller.GetType() + "." : string.Empty)}{nameof(Name)}: {Name}; {nameof(ID)}: {ID})";

                    if (MethodNameBuilder.cachedStrings.TryAdd(this, str, out var entry))
                        entry.ExpirationTimeRelativeToNow = 5.Minutes();
                }
            }

            return str;
        }
    }
}
