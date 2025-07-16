#nullable enable
namespace UTIRLib
{
    public interface IValuePair
    {
        object First { get; }
        object Second { get; }
    }
    public interface IValuePair<TFirst, TSecond> : IValuePair
    {
        new TFirst First { get; }
        new TSecond Second { get; }
    }
}
