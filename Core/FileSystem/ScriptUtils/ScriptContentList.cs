using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public sealed class ScriptContentList : IList<IScriptContent>
    {
        private readonly List<IScriptContent> list = new();

        public UsingEntry[] Usings {
            get => list.OfType<UsingEntry>().ToArray();
        }
        public bool HasUsings => Usings.Length > 0;

        public NamespaceEntry? Namespace {
            get => (NamespaceEntry)list.SingleOrDefault(x => x is NamespaceEntry);
        }
        public bool HasNamespace => Namespace is not null;

        public IType[] Types {
            get => list.OfType<IType>().ToArray();
        }
        public bool HasTypes => Types.Length > 0;

        public IScriptContent[] TypeMembers {
            get => list.OfType<IScriptContent>().ToArray();
        }
        public bool HasTypeMembers => TypeMembers.Length > 0;

        public ConstructorEntry[] Constructors {
            get => list.OfType<ConstructorEntry>().ToArray();
        }
        public bool HasConstructors => Constructors.Length > 0;

        public IField[] Fields {
            get => list.OfType<IField>().ToArray();
        }
        public bool HasFields => Fields.Length > 0;

        public IProperty[] Properties {
            get => list.OfType<IProperty>().ToArray();
        }
        public bool HasProperties => Properties.Length > 0;

        public IMethod[] Methods {
            get => list.OfType<IMethod>().ToArray();
        }
        public bool HasMethods => Methods.Length > 0;

        public int Count => list.Count;
        public IScriptContent this[int index] {
            get => list[index];
            set => list[index] = value;
        }

        bool ICollection<IScriptContent>.IsReadOnly {
            get => ((ICollection<IScriptContent>)list).IsReadOnly;
        }

        public ScriptContentList() : base()
        {
        }

        public ScriptContentList(int capacity)
        {
            list = new List<IScriptContent>(capacity);
        }

        public ScriptContentList(IEnumerable<IScriptContent> scriptContentLines)
        {
            list = new List<IScriptContent>(scriptContentLines);
        }

        public ScriptContentList(params IScriptContent[] scriptContentLines)
        {
            list = new List<IScriptContent>(scriptContentLines);
        }

        public void Add(IScriptContent item) => list.Add(item);

        public void Insert(int index, IScriptContent item) => list.Insert(index, item);

        public bool Remove(IScriptContent item) => list.Remove(item);

        public void RemoveAt(int index) => list.RemoveAt(index);

        public void Clear() => list.Clear();

        public bool Contains(IScriptContent item) => list.Contains(item);

        public int IndexOf(IScriptContent item) => list.IndexOf(item);

        public void CopyTo(IScriptContent[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public IScriptContent[] ToArray()
        {
            int listCount = list.Count;
            var array = new IScriptContent[listCount];
            for (int i = 0; i < listCount; i++)
                array[i] = list[i];

            return array;
        }

        public IEnumerator<IScriptContent> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
