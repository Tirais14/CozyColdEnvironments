using CCEnvs.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Storages
{
    public class ItemEqulaityComparer : IEqualityComparer<IItem?>
    {
        public static ItemEqulaityComparer Default { get; } = new();

        public bool Equals(IItem? x, IItem? y)
        {
            return ReferenceEquals(x, y)
                   ||
                   x.IsNotNull()
                   &&
                   y.IsNotNull()
                   &&
                   x.Name == y.Name
                   &&
                   x.ID == y.ID;
        }

        public int GetHashCode(IItem? obj)
        {
            if (obj.IsNull())
                return 0;

            return HashCode.Combine(obj.Name, obj.ID);
        }
    }
}
