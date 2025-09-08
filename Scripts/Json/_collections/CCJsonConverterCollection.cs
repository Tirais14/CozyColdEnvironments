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

        public void AddOrReplaceAllByType(JsonConverter converter)
        {
            Validate.ArgumentNull(converter, nameof(converter));

            if (!ReplaceAllByType(converter))
                Add(converter);
        }

        public bool ReplaceAllByType(JsonConverter converter)
        {
            Validate.ArgumentNull(converter, nameof(converter));

            Type conversationType = GetConversationType(converter);
            if (!RemoveAllByType(conversationType))
                return false;

            Add(converter);
            return true;
        }

        public bool RemoveAllByType(Type conversationType)
        {
            Validate.ArgumentNull(conversationType, nameof(conversationType));

            IndexValuePair<JsonConverter>[] withSameConversationType = FindAllByType(conversationType);

            if (withSameConversationType.IsEmpty())
                return false;

            foreach (var pair in withSameConversationType)
                RemoveItem(pair.index);

            return true;
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
