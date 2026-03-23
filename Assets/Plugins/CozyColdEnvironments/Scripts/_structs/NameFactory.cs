using System;
using System.Runtime.CompilerServices;
using CCEnvs.Caching;
using Humanizer;

#nullable enable
namespace CCEnvs
{
    public static class NameFactory
    {
        private static Lazy<Cache<(Type Type, int CallerHash), string>> names = new(
            static () =>
            {
                return new Cache<(Type Type, int CallerHash), string>();
            });

        public static string CreateFromCaller<TCaller>(
            TCaller? caller,
            string? body,
            Identifier? id = null,
            bool addHashToId = true,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            if (caller.IsNull())
                return $"{body} - {id.GetValueOrDefault()}";

            var nameKey = (Type: caller.GetType(), CallerHash: caller.GetHashCode());

            if (names.Value.TryGetValue(nameKey, out var name))
                return name;

            if (id == null)
                id = nameKey.CallerHash;
            else if (addHashToId)
                id = id.Value.WithNumber(id.Value.Number + nameKey.CallerHash);

            name = $"{caller.GetType()}.{body} - {id}";

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
