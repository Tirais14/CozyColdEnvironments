using System;

#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public abstract record PropertyMethod : ScriptEntry, IMethod
    {
        public IScriptContent[] BodyLines { get; set; } = Array.Empty<IScriptContent>();
        public Type? TypeValue { get; set; }
        public bool ByLambda { get; set; }

        IScriptContent[] IContentProvider.Content {
            get => BodyLines;
            set => BodyLines = value;
        }
        bool IContentProvider.HasContent => BodyLines.IsNotNullOrEmpty();
    }
}
