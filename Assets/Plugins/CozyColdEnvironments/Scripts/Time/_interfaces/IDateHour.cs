#nullable enable
namespace CCEnvs.Dates
{
    public interface IDateHour
    {
        IDateMinute this[int number] { get; }

        int Value { get; }
    }

    public interface IDateHour<T> : IDateHour
        where T : IDateMinute
    {
        new T this[int number] { get; }

        IDateMinute IDateHour.this[int number] => this[number];
    }
}
