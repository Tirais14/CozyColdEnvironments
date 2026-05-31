using CCEnvs.Serialization;
using System;
using System.Runtime.Serialization;

#nullable enable
namespace CCEnvs.Dates
{
    [Serializable, DataContract]
    public struct TimeSpanFloatDTO : IDataTransferObject<TimeSpanFloat>
    {
        [DataMember(Name = "milliseconds")]
        public int Milliseconds;

        [DataMember(Name = "seconds")]
        public float Seconds;

        [DataMember(Name = "minutes")]
        public float Minutes;

        [DataMember(Name = "hours")]
        public float Hours;

        public readonly TimeSpanFloat Materialize()
        {
            var time = TimeSpanFloat.Empty;

            if (Hours > 0f)
                time += TimeSpanFloat.FromHours(Hours);

            if (Seconds > 0f)
                time += new TimeSpanFloat(Seconds);

            if (Minutes > 0f)
                time += TimeSpanFloat.FromMinutes(Minutes);

            if (Milliseconds > 0)
                time += TimeSpanFloat.FromMilliseconds(Milliseconds);

            return time;
        }
    }
}
