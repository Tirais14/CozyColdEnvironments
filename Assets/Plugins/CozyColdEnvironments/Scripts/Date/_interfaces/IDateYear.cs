#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Dates
{
    public interface IDateYear
    {
        IDateMonth this[int number] { get; }

        int MonthCount { get; }
        int Value { get; }

        bool TryGetMonth(int number, [NotNullWhen(true)] out IDateMonth? month);
    }

    public interface IDateYear<T> : IDateYear
        where T : IDateMonth
    {
        new T this[int number] { get; }

        IDateMonth IDateYear.this[int number] => this[number];

        bool TryGetMonth(int number, [NotNullWhen(true)] out T? month);

        bool IDateYear.TryGetMonth(int number, [NotNullWhen(true)] out IDateMonth? month)
        {
            if (!TryGetMonth(number, out var typed))
            {
                month = null;
                return false;
            }

            month = typed;
            return true;
        }
    }
}
