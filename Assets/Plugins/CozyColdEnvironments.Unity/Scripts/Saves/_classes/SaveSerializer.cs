using CCEnvs.Collections;
using Newtonsoft.Json;
using System;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public static class SaveSerializer
    {
        public static SaveData[] Deserialize(string serialized)
        {
            if (serialized.IsNullOrWhiteSpace())
            {
                //TODO: Remove extra null validation
                if(serialized is null)
                    typeof(SaveSerializer).PrintError($"Argument {serialized} is null");

                return new arr<SaveData>();
            }

            try
            {
                var saveDatas = JsonConvert.DeserializeObject<SaveData[]>(serialized);

                if (saveDatas is null)
                {
                    typeof(SaveSerializer).PrintWarning($"Nothing deserialized from: {Environment.NewLine}{serialized}");
                    
                    return new arr<SaveData>();
                }

                return saveDatas;
            }
            catch (Exception ex)
            {
                typeof(SaveSerializer).PrintException(ex);

                return new arr<SaveData>();
            }
        }

        public static string Serialize(SaveData[] saveDatas)
        {
            if (saveDatas.IsNull())
            {
                typeof(SaveSerializer).PrintError($"Argument: {nameof(saveDatas)} is null");

                return string.Empty;
            }

            if (saveDatas.IsEmpty())
                return string.Empty;

            try
            {
                var serialized = JsonConvert.SerializeObject(saveDatas, CC.JsonSettings);

                return serialized;
            }
            catch (Exception ex)
            {
                typeof(SaveSerializer).PrintException(ex);

                return string.Empty;
            } 
        }
    }
}
