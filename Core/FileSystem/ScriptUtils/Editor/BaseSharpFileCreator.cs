#nullable enable

using UTIRLib.Collections;
using UTIRLib.Extensions;
using UTIRLib.FileSystem.ScriptUtils;

namespace UTIRLib.FileSystem.Editor
{
    public abstract class BaseSharpFileCreator<T>
    {
        protected readonly T[] values;
        protected readonly NamespaceEntry? namespaceData;

        public bool IsEnum { get; set; }

        protected bool HasNamespace => namespaceData is not null;

        protected BaseSharpFileCreator(T[] values, bool isEnum, params string[] namespaceParts)
        {
            this.values = values;
            IsEnum = isEnum;

            if (namespaceParts.IsNotNullOrEmpty())
            {
                namespaceData = new NamespaceEntry(namespaceParts);
            }
        }

        protected BaseSharpFileCreator(T value, bool isEnum, params string[] namespaceParts) :
            this(ArrayFactory.Create(value), isEnum, namespaceParts)
        {
        }

        public abstract ScriptFile[] GetFiles();
    }
}