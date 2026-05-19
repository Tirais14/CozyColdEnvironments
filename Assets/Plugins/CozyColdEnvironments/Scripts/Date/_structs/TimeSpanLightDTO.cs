using CCEnvs.Serialization;
using System;
using System.Runtime.Serialization;

#nullable enable
namespace CCEnvs.Dates
{
    [Serializable, DataContract]
    public struct TimeSpanLightDTO : IDataTransferObject<TimeSpanLight>
    {
        [DataMember(Name = "milliseconds")]
        public int Milliseconds;

        [DataMember(Name = "seconds")]
        public float Seconds;

        [DataMember(Name = "minutes")]
        public float Minutes;

        [DataMember(Name = "hours")]
        public float Hours;

        public readonly TimeSpanLight Materialize()
        {
            var time = TimeSpanLight.Empty;

            if (Hours > 0f)
                time += TimeSpanLight.FromHours(Milliseconds);

            if (Seconds > 0f)
                time += new TimeSpanLight(Seconds);

            if (Minutes > 0f)
                time += TimeSpanLight.FromMinutes(Milliseconds);

            if (Milliseconds > 0)
                time += TimeSpanLight.FromMilliseconds(Milliseconds);

            return time;
        }
    }
}
