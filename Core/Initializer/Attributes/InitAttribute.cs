using System;

namespace UTIRLib.Attributes
{
    /// <summary>
    /// Uses for mark method, which will be invoked by <see cref="IInitializable"/> interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class InitAttribute : Attribute
    {
        public int Order { get; set; }

        public InitAttribute(int order = 0) => Order = order;
    }
}