using System;
using System.Collections.Generic;
using System.Linq;
using UTIRLib.Attributes.Metadata;
using UTIRLib.Diagnostics;
using static UTIRLib.FileSystem.ScriptUtils.Syntax;

#nullable enable
namespace UTIRLib.FileSystem.ScriptUtils
{
    public sealed record ConstructorEntry : ScriptEntry,
        IMethod,
        ITypeMember, 
        IArgumentsDefineProvider
    {
        [Flags]
        public enum Modifiers
        {
            [MetaString("")]
            None,

            [MetaString("partial")]
            Partial = 2,

            [MetaString("static")]
            Static = 4,
        }

        public enum Inheritance
        {
            [MetaString("")]
            None,

            [MetaString("this")]
            This,

            [MetaString("base")]
            Base
        }

        public AccessModifier AccessModifier { get; set; } = AccessModifier.Private;
        public Modifiers ModifierFlags { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public ArgumentDefineEntry[] DefineArguments 
            { get; set; } = Array.Empty<ArgumentDefineEntry>();
        public bool HasDefineArguments => DefineArguments.IsNotNullOrEmpty();
        public ConstructorEntry? Parent { get; set; }
        public Inheritance InheritanceType { get; set; }
        public IScriptContent[] BodyLines { get; set; } = Array.Empty<IScriptContent>();
        public bool ByLambda { get; set; }

        IScriptContent[] IContentProvider.Content {
            get => BodyLines;
            set => BodyLines = value;
        }
        bool IContentProvider.HasContent => BodyLines.IsNotNullOrEmpty();

        public ConstructorEntry(Type constructableType) : base()
        {
            TabulationsCount = 2;
            TypeName = constructableType.GetProccessedName(TypeNameAttributes.ShortName);
        }
        public ConstructorEntry() : base()
        {
            TabulationsCount = 2;
        }

        public static ConstructorEntry Create<T>()
        {
            return new ConstructorEntry(typeof(T));
        }

        public override string ToString() => base.ToString();

        protected override void BuildString()
        {
            WriteWithWhitespace(Attributes);

            WriteWithWhitespace(AccessModifier);
            WriteWithWhitespace(ModifierFlags);

            Write(TypeName);
            Write('(');
            Write(DefineArguments);

            if (TryWriteParentConstructor())
                WriteWithWhitespace(')');
            else
            {
                if (ByLambda)
                    WriteWithWhitespace(')');
                else
                    WriteLine(')');
            }


            TryWriteParentConstructor();

            if (ByLambda)
            {
                Write("=> ");

                Write(BodyLines);

                Write(';');
            }
            else
            {
                WriteLine('{');

                WriteLine(BodyLines);

                Write('}');
            }
        }

        private bool TryWriteParentConstructor()
        {
            if (Parent is null || InheritanceType == Inheritance.None)
                return false;

            string parentArguments = GetParentArguments();

            Write(InheritanceType);

            Write('(');

            Write(parentArguments);

            Write(')');

            return true;
        }

        /// <exception cref="ArgumentException"></exception>
        private string GetParentArguments()
        {
            string[] parentArgumentNames = Parent!.DefineArguments.Select(x => x.ArgumentName).ToArray();

            if (parentArgumentNames.IsEmpty())
                return string.Empty;

            string[] argumentNames = DefineArguments.Select(x => x.ArgumentName).ToArray();

            if (argumentNames.Length < parentArgumentNames.Length)
                throw new ArgumentException("Parent constructor couldn't be have less arguments count than children.");

            var results = new List<string>(parentArgumentNames.Length);

            for (int i = 0; i < parentArgumentNames.Length; i++)
            {
                if (argumentNames.Contains(parentArgumentNames[i]))
                    results.Add(parentArgumentNames[i]);
            }

            TirLibDebug.AssertWarning(results.Count < parentArgumentNames.Length, "Not all arguments found.");

            return results.JoinStrings(", ");
        }
    }
}
