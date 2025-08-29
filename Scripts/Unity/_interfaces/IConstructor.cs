#nullable enable
namespace CozyColdEnvironments
{
    public interface IConstructor
    {
        bool IsConstructed { get; }
    }
    public interface IConstructor<in T> : IConstructor
    {
        void Construct(T value);
    }
    public interface IConstructor<in T0, in T1> : IConstructor
    {
        void Construct(T0 value, T1 value1);
    }
    public interface IConstructor<in T0, in T1, in T2> : IConstructor
    {
        void Construct(T0 value, T1 value1, T2 value2);
    }
    public interface IConstructor<in T0, in T1, in T2, in T3> : IConstructor
    {
        void Construct(T0 value,
                       T1 value1,
                       T2 value2,
                       T3 value3);
    }
    public interface IConstructor<in T0, in T1, in T2, in T3, in T4> : IConstructor
    {
        void Construct(T0 value,
                       T1 value1,
                       T2 value2,
                       T3 value3,
                       T4 value4);
    }
    public interface IConstructor<in T0, in T1, in T2, in T3, in T4, in T5> : IConstructor
    {
        void Construct(T0 value,
                       T1 value1,
                       T2 value2,
                       T3 value3,
                       T4 value4,
                       T5 value5);
    }
}
