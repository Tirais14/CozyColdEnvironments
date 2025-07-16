#nullable enable
using System;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public readonly struct ScriptContentPriority
    {
        public readonly Type Type { get; }
        public readonly int Value { get; }

        private ScriptContentPriority(Type type, int priority)
        {
            Type = type;
            Value = priority;
        }

        public static ScriptContentPriority Create<T>(int priority)
            where T : IScriptContent
        {
            return new ScriptContentPriority(typeof(T), priority);
        }
    }
}
