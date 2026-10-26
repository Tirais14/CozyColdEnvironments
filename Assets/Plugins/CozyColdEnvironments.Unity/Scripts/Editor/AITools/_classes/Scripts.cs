#if UNITY_EDITOR
using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace CCEnvs.UnityX.Editr
{
    public class Scripts
    {
        private readonly Dictionary<string, string> byPathContents = new();
        private readonly Dictionary<string, List<string>> byNamespaceContents = new();

        public object Gate { get; } = new();

        public void Add(string filePath, string ns, string content)
        {
            Guard.IsNotNullOrWhiteSpace(filePath, nameof(filePath));
            Guard.IsNotNull(ns);
            Guard.IsNotNull(content);

            lock (Gate)
            {
                byPathContents[filePath] = content;
                byNamespaceContents.GetOrCreateNew(ns).Add(content);
            }
        }

        public string GetByPath(string filePath)
        {
            lock (Gate)
            {
                return byPathContents.TryGetValue(filePath, out var content) ? content : string.Empty;
            }
        }

        public IReadOnlyList<string> GetByNamespace(string ns)
        {
            lock (Gate)
            {
                if (!byNamespaceContents.TryGetValue(ns, out var contents))
                    return Array.Empty<string>();

                return contents.ToList();
            }
        }

        // ✅ Обязательно добавьте этот метод, чтобы GenerateAsync мог прочитать результаты
        public IEnumerable<KeyValuePair<string, string>> GetAll()
        {
            lock (Gate)
            {
                return byPathContents.ToList();
            }
        }
    }
}
#endif