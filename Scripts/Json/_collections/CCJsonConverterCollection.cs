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
        private readonly List<KeyValuePair<int, JsonConverter>> values = new(0);

        public int Count => values.Count;
        public bool IsReadOnly => false;
        public JsonConverter this[int index] {
            get => values[index].Value;
            set => values[index] = new KeyValuePair<int, JsonConverter>(value.GetType().GetBaseTypeCount(), value);
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
                if (GetConversationType(values[i].Value) == conversationType)
                    results.Add(new IndexValuePair<JsonConverter>(i, values[i].Value));
            }

            return results.ToArray();
        }

        public void Add(JsonConverter converter)
        {
            values.Add(new KeyValuePair<int, JsonConverter>(converter.GetType().GetBaseTypeCount(), converter));
        }

        public void Clear() => values.Clear();

        public int IndexOf(JsonConverter item) => values.IndexOf(new KeyValuePair<int, JsonConverter>(item.GetType().GetBaseTypeCount(), item));

        public bool Contains(JsonConverter item) => values.Contains(new KeyValuePair<int, JsonConverter>(item.GetType().GetBaseTypeCount(), item));

        public bool Remove(JsonConverter item)
        {
            return values.Remove(new KeyValuePair<int, JsonConverter>(item.GetType().GetBaseTypeCount(), item));
        }
        public void RemoveAt(int index) => values.RemoveAt(index);

        public IEnumerator<JsonConverter> GetEnumerator()
        {
            return values.Select(x => x.Value)
                         .ToArray()
                         .GetEnumeratorT();
        }

        void ICollection<JsonConverter>.CopyTo(JsonConverter[] array, int arrayIndex)
        {
            values.Select(x => x.Value)
                  .ToArray()
                  .CopyTo(array, arrayIndex);
        }

        void IList<JsonConverter>.Insert(int index, JsonConverter item)
        {
            values.Index()
                  .Insert(index, new KeyValuePair<int, JsonConverter>(item.GetType().GetBaseTypeCount(), item))
                  .Unindex()
                  .Materialize()
                  .Before(values, x => x.Clear())
                  .Aggregate(values, (list, x) => { list.Add(x); return list; })
                  .ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
