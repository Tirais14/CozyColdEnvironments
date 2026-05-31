using CCEnvs.Collections;
using CCEnvs.Serialization;
using System;
using System.Runtime.Serialization;

#nullable enable
namespace CCEnvs.Dates
{
    [Serializable, DataContract]
    public struct DateTimeCustomDTO : IDataTransferObject<DateTimeCustom>
    {
        [DataMember(Name = "calendar")]
        public StructuralArray<int> Calendar;

        [DataMember(Name = "year")]
        public int Year;

        [DataMember(Name = "month")]
        public int Month;

        [DataMember(Name = "day")]
        public int Day;

        [DataMember(Name = "time")]
        public TimeSpanFloatDTO Time;

        public readonly DateTimeCustom Materialize()
        {
            return new DateTimeCustom(
                Year,
                Month,
                Day,
                Time.Materialize(),
                Calendar
                );
        }
    }
}
