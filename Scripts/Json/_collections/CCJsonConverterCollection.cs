using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Linq;
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
    public class CCJsonConverterCollection 
        : IList<JsonConverter>, 
        IReadOnlyList<JsonConverter>
    {
        private readonly SortedList<int, JsonConverter> values = new();

        public int Count => values.Count;
        public bool IsReadOnly => false;
        public JsonConverter this[int index] {
            get => values[index];
            set => values[index] = value;
        }

        public CCJsonConverterCollection()
        {
        }

        public CCJsonConverterCollection(int capacity)
        {
            values = new SortedList<int, JsonConverter>(capacity);
        }

        public CCJsonConverterCollection(IEnumerable<JsonConverter> converters)
            :
            this()
        {
            this.AddRange(converters);
        }

        public CCJsonConverterCollection AddOrReplaceAllByType(JsonConverter converter)
        {
            CC.Validate.ArgumentNull(converter, nameof(converter));

            ReplaceAllByType(converter, out bool isReplaced);

            if (!isReplaced)
                Add(converter);

            return this;
        }

        public CCJsonConverterCollection ReplaceAllByType(JsonConverter converter,
                                                          out bool isReplaced)
        {
            CC.Validate.ArgumentNull(converter, nameof(converter));

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
            CC.Validate.ArgumentNull(conversationType, nameof(conversationType));

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
            CC.Validate.ArgumentNull(conversationType, nameof(conversationType));

            var results = new List<IndexValuePair<JsonConverter>>();
            int count = values.Count;
            for (int i = 0; i < count; i++)
            {
                if (GetConversationType(values[i]) == conversationType)
                    results.Add(new IndexValuePair<JsonConverter>(i, values[i]));
            }

            return results.ToArray();
        }

        public void Add(JsonConverter converter)
        {
            values.Add(converter.GetType().GetBaseTypeCount(), converter);
        }

        public void Clear() => values.Clear();

        public int IndexOf(JsonConverter item) => values.IndexOfValue(item);

        public bool Contains(JsonConverter item) => values.ContainsValue(item);

        public bool Remove(JsonConverter item)
        {
            return values.Remove(item.GetType().GetBaseTypeCount());
        }
        public void RemoveAt(int index) => values.RemoveAt(index);

        public IEnumerator<JsonConverter> GetEnumerator()
        {
            return values.Select(x => x.Value).ToArray().GetEnumeratorT();
        }

        void ICollection<JsonConverter>.CopyTo(JsonConverter[] array, int arrayIndex)
        {
            values.Select(x => x.Value).ToArray().CopyTo(array, arrayIndex);
        }

        void IList<JsonConverter>.Insert(int index, JsonConverter item)
        {
            IEnumerable<KeyValuePair<int, JsonConverter>> result =
                values.Index()
                      .Insert(index, new KeyValuePair<int, JsonConverter>(item.GetType().GetBaseTypeCount(), item))
                      .Unindex()
                      .ToArray();

            values.Clear();
            foreach (var value in result)
                values.Add(value.Key, value.Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
