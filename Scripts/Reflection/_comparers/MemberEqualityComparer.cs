#nullable enable
using CCEnvs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CCEnvs.Reflection
{
    public class MemberEqualityComparer 
        : IEqualityComparer<FieldInfo>, 
        IEqualityComparer<EventInfo>,
        IEqualityComparer<PropertyInfo>,
        IEqualityComparer<MethodInfo>
    {
        public bool Equals(FieldInfo x, FieldInfo y)
        {
            return x.Name == y.Name;
        }

        public bool Equals(EventInfo x, EventInfo y)
        {
            return x.Name == y.Name;
        }

        public bool Equals(PropertyInfo x, PropertyInfo y)
        {
            return x.Name == y.Name
                   && 
                   x.GetIndexParameters().SequenceEqual(y.GetIndexParameters());
        }

        public bool Equals(MethodInfo x, MethodInfo y)
        {
            return x.Name == y.Name
                   &&
                   x.GetParameters().SequenceEqual(y.GetParameters())
                   &&
                   x.GetGenericArguments().SequenceEqual(y.GetGenericArguments());
        }

        public int GetHashCode(FieldInfo obj)
        {
            return HashCode.Combine(obj.Name);
        }

        public int GetHashCode(EventInfo obj)
        {
            return HashCode.Combine(obj.Name);
        }

        public int GetHashCode(PropertyInfo obj)
        {
            return HashCode.Combine(obj.Name,
                                    obj.GetIndexParameters().AsCCParameters());
        }

        public int GetHashCode(MethodInfo obj)
        {
            return HashCode.Combine(obj.Name,
                                    obj.GetParameters().AsCCParameters(),
                                    obj.GetGenericArguments().Select(x => x.Name).SequenceToHashCode());
        }
    }
}
