using System;

namespace UTIRLib.Collections
{
    public struct PriorityValuePair<TPriority, TValue> : IComparable<PriorityValuePair<TPriority, TValue>>
        where TPriority : struct, IComparable<TPriority>
    {
        public TPriority priority;
        public TValue value;

        public PriorityValuePair(TPriority priority, TValue value)
        {
            this.priority = priority;
            this.value = value;
        }

        public int CompareTo(PriorityValuePair<TPriority, TValue> other) => priority.CompareTo(other.priority);
    }
}