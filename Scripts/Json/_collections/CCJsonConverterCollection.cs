using CCEnvs.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static CCEnvs.Json.JsonConverterHelper;

#nullable enable
namespace CCEnvs.Json
{
    public class CCJsonConverterCollection : Collection<JsonConverter>
    {
        public CCJsonConverterCollection()
        {
        }

        public CCJsonConverterCollection(IList<JsonConverter> list) : base(list)
        {
        }

        public CCJsonConverterCollection(IEnumerable<JsonConverter> collection)
            :
            base(collection.ToArray())
        {
        }

        public CCJsonConverterCollection AddOrReplaceAllByType(JsonConverter converter)
        {
            Validate.ArgumentNull(converter, nameof(converter));

            ReplaceAllByType(converter, out bool isReplaced);

            if (!isReplaced)
                Add(converter);

            return this;
        }

        public CCJsonConverterCollection ReplaceAllByType(JsonConverter converter,
                                                          out bool isReplaced)
        {
            Validate.ArgumentNull(converter, nameof(converter));

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
            Validate.ArgumentNull(conversationType, nameof(conversationType));

            IndexValuePair<JsonConverter>[] withSameConversationType = FindAllByType(conversationType);

            if (withSameConversationType.IsEmpty())
                isRemoved = false;

            foreach (var pair in withSameConversationType)
                RemoveItem(pair.index);

            isRemoved = true;

            return this;
        }

        public IndexValuePair<JsonConverter>[] FindAllByType(Type conversationType)
        {
            Validate.ArgumentNull(conversationType, nameof(conversationType));

            var results = new List<IndexValuePair<JsonConverter>>();
            int count = Items.Count;
            for (int i = 0; i < count; i++)
            {
                if (GetConversationType(Items[i]) == conversationType)
                    results.Add(new IndexValuePair<JsonConverter>(i, Items[i]));
            }

            return results.ToArray();
        }
    }
}
