//using CCEnvs.Reflection;
//using CCEnvs.Saves;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using ObservableCollections;
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//#nullable enable
//namespace CCEnvs.Json.Converters
//{
//    public class SaveDictionaryJsonConverter : JsonConverter
//    {
//        public override bool CanWrite => false;

//        public override bool CanConvert(Type objectType)
//        {
//            return objectType.IsType<>
//        }

//        public override ObservableDictionary<string, SaveEntry>? ReadJson(
//            JsonReader reader,
//            Type objectType,
//            ObservableDictionary<string, SaveEntry>? existingValue,
//            JsonSerializer serializer
//            )
//        {
//            if (reader.TokenType != JsonToken.Null)
//                return null;

//            var jObj = JObject.Load(reader);

//            var dictionary = new ObservableDictionary<string, SaveEntry>();

//            foreach (var jProp in jObj.Properties())
//            {
//                serializer.Deserialize(jProp)
//            }
//        }

//        public override void WriteJson(
//            JsonWriter writer,
//            ObservableDictionary<string, SaveEntry>? value,
//            JsonSerializer serializer
//            )
//        {
//        }
//    }
//}
