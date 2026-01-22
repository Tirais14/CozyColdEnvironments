using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public class CommandEqualityComparer : IEqualityComparer<ICommandBase>
    {
        public bool Equals(ICommandBase x, ICommandBase y)
        {
            return ReferenceEquals(x, y)
                   ||
                   (x.Name == y.Name
                   &&
                   x.CommandType == y.CommandType);
        }

        public int GetHashCode(ICommandBase obj)
        {
            return HashCode.Combine(obj.Name, obj.CommandType);
        }
    }
}
