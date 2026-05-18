#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Dates
{
    public interface IDateMonth
    {
        IDateDay this[int number] { get; }

        int DayCount { get; }
        int Value { get; }

        bool TryGetDay(int number, [NotNullWhen(true)] out IDateDay? day);
    }

    public interface IDateMonth<T> : IDateMonth
        where T : IDateDay
    {
        new T this[int number] { get; }

        IDateDay IDateMonth.this[int number] => this[number];

        bool TryGetDay(int number, [NotNullWhen(true)] out T? day);

        bool IDateMonth.TryGetDay(int number, [NotNullWhen(true)] out IDateDay? day)
        {
            if (!TryGetDay(number, out var typed))
            {
                day = null;
                return false;
            }

            day = typed;
            return true;
        }
    }
}
