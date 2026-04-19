using System;
using System.Runtime.CompilerServices;
using CCEnvs.Caching;
using CCEnvs.Pools;
using CCEnvs.Reflection.Caching;
using Humanizer;

#nullable enable
namespace CCEnvs
{
    public static class NameFactory
    {
        private readonly static Lazy<Cache<(Type Type, Identifier? ID, int CallerHash), string>> names = new(
            static () =>
            {
                return new Cache<(Type Type, Identifier? ID, int CallerHash), string>();
            });

        public static string CreateFromCaller<TCaller>(
            TCaller? caller,
            [CallerMemberName] string? body = "Unkwown",
            Identifier? id = null,
            bool addHashToId = true,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            using var stringBuilder = StringBuilderPool.Shared.Get();

            if (caller.IsNull())
            {
                stringBuilder.Value.Append(body);
                stringBuilder.Value.Append(" - ");
                stringBuilder.Value.Append(id.GetValueOrDefault());

                return stringBuilder.Value.ToString();
            }

            var nameKey = (Type: caller.GetType(), id, CallerHash: caller.GetHashCode());

            if (names.Value.TryGetValue(nameKey, out var name))
                return name;

            if (id == null)
                id = nameKey.CallerHash;
            else if (addHashToId)
                id = id.Value.WithNumber(id.Value.Number + nameKey.CallerHash);

            stringBuilder.Value.Append(TypeCache.GetName(caller.GetType()));
            stringBuilder.Value.Append('.');
            stringBuilder.Value.Append(body);
            stringBuilder.Value.Append(" - ");
            stringBuilder.Value.Append(id.ToString());

            name = stringBuilder.Value.ToString();

            expirationTimeRelativeToNow ??= 5.Minutes();

            if (expirationTimeRelativeToNow.HasValue
                &&
                names.Value.TryAdd(nameKey, name, out var entry))
            {
                entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow;
            }

            return name;
        }
    }
}
