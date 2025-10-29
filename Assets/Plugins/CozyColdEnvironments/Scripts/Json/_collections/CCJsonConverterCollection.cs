using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static CCEnvs.Json.JsonConverterHelper;

#nullable enable
namespace CCEnvs.Json
{
    [Obsolete("In developing")]
    public class CCJsonConverterCollection 
        : IList<JsonConverter>, 
        IReadOnlyList<JsonConverter>
    {
        private readonly List<KeyValuePair<int, JsonConverter>> values = new(0);

        public int Count => values.Count;
        public bool IsReadOnly => false;
        public JsonConverter this[int index] {
            get => values[index].Value;
            set => values[index] = ToListItem(value);
        }

        public CCJsonConverterCollection()
        {
        }

        public CCJsonConverterCollection(int capacity)
        {
            values = new List<KeyValuePair<int, JsonConverter>>(capacity);
        }

        public CCJsonConverterCollection(IEnumerable<JsonConverter> converters)
            :
            this()
        {
            this.AddRange(converters);
        }

        private static int GetPriority(JsonConverter converter)
        {
            try
            {
                Type conversationType = GetConversationType(converter);

                return conversationType.GetParentsCount();
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static KeyValuePair<int, JsonConverter> ToListItem(JsonConverter converter)
        {
            return new KeyValuePair<int, JsonConverter>(GetPriority(converter), converter);
        }

        public CCJsonConverterCollection AddOrReplaceAllByType(JsonConverter converter)
        {
            CC.Guard.IsNotNull(converter, nameof(converter));

            ReplaceAllByType(converter, out bool isReplaced);

            if (!isReplaced)
                Add(converter);

            return this;
        }

        public CCJsonConverterCollection ReplaceAllByType(JsonConverter converter,
                                                          out bool isReplaced)
        {
            CC.Guard.IsNotNull(converter, nameof(converter));

            Type conversationType = GetConversationType(converter);
            RemoveAllByType(conversationType, out isReplaced);

            if (!isReplaced)
                return this;

            Add(converter);
            isReplaced = true;

            return this;
        }

        public CCJsonConverterCollection RemoveAllByType(Type conversationType,
                                                         out bool isRemoved)
        {
            CC.Guard.IsNotNull(conversationType, nameof(conversationType));

            IndexValuePair<JsonConverter>[] withSameConversationType = FindAllByType(conversationType);

            if (withSameConversationType.IsEmpty())
                isRemoved = false;

            foreach (var pair in withSameConversationType)
                values.RemoveAt(pair.index);

            isRemoved = true;

            return this;
        }

        public IndexValuePair<JsonConverter>[] FindAllByType(Type conversationType)
        {
            CC.Guard.IsNotNull(conversationType, nameof(conversationType));

            var results = new List<IndexValuePair<JsonConverter>>();
            int count = values.Count;
            for (int i = 0; i < count; i++)
            {
                if (GetConversationType(values[i].Value) == conversationType)
                    results.Add(new IndexValuePair<JsonConverter>(i, values[i].Value));
            }

            return results.ToArray();
        }

        public void Add(JsonConverter converter)
        {
            values.Add(ToListItem(converter));
            Reorder();
        }

        public void Clear() => values.Clear();

        public int IndexOf(JsonConverter item)
        {
            return values.IndexOf(ToListItem(item));
        }

        public bool Contains(JsonConverter item)
        {
            return values.Contains(ToListItem(item));
        }

        public bool Remove(JsonConverter item)
        {
            bool result = values.Remove(ToListItem(item));
            Reorder();
            return result;
        }
        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
            Reorder();
        }

        public IEnumerator<JsonConverter> GetEnumerator()
        {
            return values.Select(x => x.Value)
                         .ToArray()
                         .GetEnumeratorT();
        }

        private void Reorder()
        {
            values.Sort((x, y) => x.Key.CompareTo(y.Key));
        }

        void ICollection<JsonConverter>.CopyTo(JsonConverter[] array, int arrayIndex)
        {
            values.Select(x => x.Value)
                  .ToArray()
                  .CopyTo(array, arrayIndex);
        }

        void IList<JsonConverter>.Insert(int index, JsonConverter item)
        {
            values.Insert(index, ToListItem(item));
            Reorder();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
