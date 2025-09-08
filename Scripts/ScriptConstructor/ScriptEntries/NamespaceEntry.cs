using CCEnvs.Common;
using CCEnvs.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.FileSystem.ScriptUtils
{
    public record NamespaceEntry : ScriptEntry, IContentProvider
    {
        public string NamespaceName { get; set; } = string.Empty;
        public IScriptContent[] Content { get; set; } = Array.Empty<IScriptContent>();
        public bool HasContent => Content.IsNotNullOrEmpty();

        public NamespaceEntry(params string[] nameParts) : base()
        {
            SetName(nameParts);
        }

        public void SetName(params string[] nameParts)
        {
            if (nameParts is null)
                throw new ArgumentNullException(nameof(nameParts));
            if (nameParts.IsEmpty())
                NamespaceName = string.Empty;

            NamespaceName = nameParts.JoinStrings(".");
        }

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            CCDebug.AssertError(NamespaceName.IsNotNullOrEmpty(), "Trying to create namespace without name.", this);

            WriteLine($"namespace {NamespaceName}");

            WriteLine("{");

            WriteLine(Content, tabulationsCount: 0);

            Write("}");
        }
    }
}