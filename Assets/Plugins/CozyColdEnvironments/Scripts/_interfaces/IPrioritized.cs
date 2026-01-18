using System;

namespace CCEnvs
{
    public interface IPrioritized
    {
        //object Priority { get; }
    }
    public interface IPrioritized<out T> : IPrioritized 
        where T : IComparable<T>
    { 
        //new T Priority { get; }

        //object IPrioritized.Priority => Priority;
    }
}
