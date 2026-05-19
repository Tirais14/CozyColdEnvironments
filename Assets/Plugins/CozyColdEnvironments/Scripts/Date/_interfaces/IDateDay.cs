using CCEnvs.Dates;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.Dates
{
    public interface IDateDay
    {
        IDateHour this[int number] { get; }

        int HourCount { get; }

        bool TryGetHour(int number, [NotNullWhen(true)] out IDateHour? hour);
    }

    public interface IDateDay<T>
        where T : IDateHour
    {
        T this[int number] { get; }

        int HourCount { get; }

        bool TryGetHour(int number, [NotNullWhen(true)] out T? hour);
    }
}
