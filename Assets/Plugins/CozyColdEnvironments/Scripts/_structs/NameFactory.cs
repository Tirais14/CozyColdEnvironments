using CCEnvs.Caching;
using CCEnvs.Pools;
using CCEnvs.Reflection.Caching;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs
{
    public static class NameFactory
    {
        private readonly static Lazy<Cache<(Type? Type, Identifier? ID, int? CallerHash, string? Body), string>> names = new(() => new());

        public static string CreateFromCaller<TCaller>(
            TCaller? caller,
            [CallerMemberName] string? body = "Unkwown",
            Identifier? id = null,
            bool addCallerHashCodeToID = false
            )
        {
            if (addCallerHashCodeToID &&
                caller.IsNotNull())
            {
                if (id == null)
                    id = caller.GetHashCode();
                else if (addCallerHashCodeToID)
                {
                    unchecked
                    {
                        id = id.Value.WithNumber(id.Value.Number + caller.GetHashCode());
                    }
                }
            }

            return CreateNameFromCaller(caller, body, id);
        }

        public static string CreateFromCallerCached<TCaller>(
            TCaller? caller,
            [CallerMemberName] string? body = "Unkwown",
            Identifier? id = null,
            bool addCallerHashCodeToID = false,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Type? callerType = null;
            int? callerHashCode = null;

            if (caller is not null)
            {
                callerType = caller.GetType();
                callerHashCode = caller.GetHashCode();
            }

            var nameKey = (Type: callerType, id, CallerHash: callerHashCode, body);

            if (NameFactory.names.TryGetValue(out var names) &&
                names.TryGetValue(nameKey, out var name))
                return name;

            if (addCallerHashCodeToID)
            {
                if (id == null)
                    id = nameKey.CallerHash;
                else if (addCallerHashCodeToID)
                {
                    unchecked
                    {
                        id = id.Value.WithNumber(id.Value.Number + nameKey.CallerHash);
                    }
                }
            }

            name = CreateNameFromCaller(caller, body, id);

            if (expirationTimeRelativeToNow.HasValue &&
                names.TryAdd(nameKey, name, out var entry))
            {
                entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow;
            }

            return name;
        }

        private static string CreateNameFromCaller<TCaller>(
            TCaller caller,
            string? body,
            Identifier? id
            )
        {
            using var stringBuilder = StringBuilderPool.Shared.Get();

            if (caller.IsNull())
            {
                if (id is null)
                    return body ?? string.Empty;

                stringBuilder.Value.Append(body);
                stringBuilder.Value.Append(':');
                stringBuilder.Value.Append(id.GetValueOrDefault());
            }
            else
            {
                stringBuilder.Value.Append(TypeCache.GetName(caller.GetType()));
                stringBuilder.Value.Append('.');
                stringBuilder.Value.Append(body);

                if (id is not null)
                {
                    stringBuilder.Value.Append(':');
                    stringBuilder.Value.Append(id.ToString());
                }
            }

            return stringBuilder.Value.ToString();
        }
    }
}
