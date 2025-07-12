#nullable enable
using System.Collections.Generic;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public interface IUsingsProvider
    {
        UsingEntry[] Usings { get; set; }
        bool HasUsings { get; }

        public void AddUsings(IEnumerable<UsingEntry> usings);
        public void AddUsings(params UsingEntry[] usings);
    }
}
