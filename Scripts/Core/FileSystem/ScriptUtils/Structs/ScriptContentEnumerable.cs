using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    /// <summary>
    /// Enumerates all nested <see cref="IScriptContent"/> lines, attributes, usings values wtih selected type
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public readonly struct ScriptContentEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<IScriptContent> scriptLines;

        public ScriptContentEnumerable(IScriptContent scriptContent)
        {
            scriptLines = scriptContent.ToArray();
        }

        public ScriptContentEnumerable(IEnumerable<IScriptContent> scriptLines)
        {
            this.scriptLines = scriptLines;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(scriptLines);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private readonly IEnumerator<IScriptContent> enumeratorInternal;

            public T Current { get; private set; }
            readonly object IEnumerator.Current => Current!;

            public Enumerator(IEnumerable<IScriptContent> scriptLines)
            {
                var list = new List<IScriptContent>();

                foreach (var line in scriptLines)
                    CollectNestedScriptContent(line, list);

                Current = default!;
                enumeratorInternal = list.GetEnumerator();
            }

            public readonly void Dispose() { }
            public bool MoveNext()
            {
                while (enumeratorInternal.MoveNext())
                {
                    if (enumeratorInternal.Current is T typed)
                    {
                        Current = typed;
                        return true;
                    }
                }

                Current = default!;
                return false;
            }
            public readonly void Reset() => enumeratorInternal.Reset();

            private static void CollectNestedScriptContent(
                IScriptContent scriptContent,
                List<IScriptContent> list)
            {
                var stack = new Stack<IScriptContent>();
                stack.Push(scriptContent);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    list.Add(current);

                    var children = new List<IScriptContent>();

                    if (current is IArgumentsProvider argumentsProvider
                        &&
                        argumentsProvider.Arguments.IsNotNullOrEmpty())
                        children.AddRange(argumentsProvider.Arguments);

                    if (current is IArgumentsDefineProvider argumentsDefineProvider 
                        &&
                        argumentsDefineProvider.DefineArguments.IsNotNullOrEmpty())
                        children.AddRange(argumentsDefineProvider.DefineArguments);

                    if (current is IAttributesProvider attributesProvider
                        &&
                        attributesProvider.Attributes.IsNotNullOrEmpty()
                        )
                        children.AddRange(attributesProvider.Attributes);

                    if (current is IUsingsProvider usingsProvider
                        &&
                        usingsProvider.Usings.IsNotNullOrEmpty()
                        )
                        children.AddRange(usingsProvider.Usings);

                    if (current is IContentProvider linesProvider
                        &&
                        linesProvider.Content.IsNotNullOrEmpty()
                        )
                        children.AddRange(linesProvider.Content);

                    for (int i = children.Count - 1; i >= 0; i--)
                        stack.Push(children[i]);
                }
            }
        }
    }
}
