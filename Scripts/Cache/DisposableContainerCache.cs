using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace UTIRLib.Disposables
{
    public static class DisposableContainerCache
    {
        private readonly static Dictionary<Type, FieldInfo> collectionFields = new();

        /// <exception cref="ArgumentNullException"></exception>
        public static FieldInfo GetCollectionField(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (collectionFields.TryGetValue(type, out FieldInfo field))
                return field;

            field = type.GetFields(BindingFlagsDefault.InstanceAll.ToBindingFlags())
                        .Single(x => x.FieldType.Is<IDisposableCollection>());

            collectionFields.Add(type, field);

            return field;
        }
    }
}
