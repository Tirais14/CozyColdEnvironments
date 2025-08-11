using System;

#nullable enable

namespace UTIRLib.FileSystem.ScriptUtils
{
    public record UsingEntry : IScriptContent
    {
        public string NamespaceValue { get; set; } = string.Empty;

        int IScriptContent.TabulationsCount { get => 0; set => _ = value; }

        public UsingEntry(params string[] nameParts) : base()
        {
            SetName(nameParts);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public void SetName(params string[] nameParts)
        {
            if (nameParts is null)
                throw new ArgumentNullException(nameof(nameParts));
            if (nameParts.Length == 0)
                return;

            if (nameParts.Length == 1)
            {
                NamespaceValue = nameParts[0];
                return;
            }

            NamespaceValue = nameParts.JoinStrings('.');
        }

        public override string ToString() => $"using {NamespaceValue};";
    }
}