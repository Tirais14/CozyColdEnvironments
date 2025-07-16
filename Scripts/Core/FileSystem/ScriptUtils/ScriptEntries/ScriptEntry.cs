#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UTIRLib.Attributes.Metadata;
using UTIRLib.EnumFlags;

namespace UTIRLib.FileSystem.ScriptUtils
{
    public abstract record ScriptEntry : IScriptContent, IAttributesProvider, IUsingsProvider
    {
        private readonly StringBuilder stringResult = new();
        private bool onLineTabulationsAdded;

        public int TabulationsCount { get; set; }
        public AttributeEntry[] Attributes { get; set; } = Array.Empty<AttributeEntry>();
        public bool HasAttributes => Attributes.IsNotNullOrEmpty();
        public UsingEntry[] Usings { get; set; } = Array.Empty<UsingEntry>();
        public bool HasUsings => Usings.IsNotNullOrEmpty();
        public IConverter<string> ValueConverter { get; set; } = new DefaultValueConverter();

        /// <exception cref="ArgumentNullException"></exception>
        public void AddUsings(IEnumerable<UsingEntry> usings)
        {
            if (Usings is null)
            {
                Usings = usings.ToArray();

                return;
            }
            if (usings is null)
                throw new ArgumentNullException(nameof(usings));

            Usings = Usings.Concat(usings).ToArray();
        }
        public void AddUsings(params UsingEntry[] usings)
        {
            AddUsings((IEnumerable<UsingEntry>)usings);
        }

        public override string ToString()
        {
            stringResult.Clear();

            BuildString();

            return stringResult.ToString();
        }

        protected string ProccessValue(object? rawValue)
        {
            if (rawValue == null)
                return string.Empty;
            if (rawValue is string str)
                return str;

            if (rawValue is IEnumerable enumerable)
            {
                StringBuilder result = new();

                string convertedValue;
                foreach (var item in enumerable)
                {
                    convertedValue = ValueConverter.Convert(item);

                    result.AppendLine(convertedValue);
                }

                return result.ToString();
            }

            return ValueConverter.Convert(rawValue);
        }

        protected abstract void BuildString();

        protected void Write(object? value, int tabulationsCount = -1)
        {
            WriteBy(value, newLine: false, tabulationsCount: tabulationsCount, addWhitespace: false);
        }

        protected void WriteWithWhitespace(object? value, int tabulationsCount = -1)
        {
            WriteBy(value, newLine: false, tabulationsCount: tabulationsCount, addWhitespace: true);
        }

        protected void WriteLine(object? value, int tabulationsCount = -1)
        {
            WriteBy(value, newLine: true, tabulationsCount: tabulationsCount, addWhitespace: false);
        }

        private static string ProccessString(string? value, bool addWhitespace)
        {
            if (value.IsNullOrEmpty())
                return string.Empty;

            if (addWhitespace && !value.EndsWith(' '))
                return value + ' ';

            return value;
        }

        private static string ProccessEnum(Enum value)
        {
            string result;

            if (value.IsFlags())
                result = value.GetMetaStringByFlags(useDefaultStringsIfNotFound: true)
                              .JoinStrings(' ');
            else
                result = value.TryGetMetaString();

            if (result.IsNullOrEmpty())
                return string.Empty;

            return result;
        }

        private string GetTabulations(int customCount = -1)
        {
            if (customCount == -1)
                customCount = TabulationsCount;

            StringBuilder result = new(customCount);

            for (int i = 0; i < customCount; i++)
                result.Append('\t');

            return result.ToString();
        }

        /// <param name="tabulationCount">if null, would be use the field value</param>
        private string ProccessAttributes(IEnumerable<AttributeEntry>? attributes,
                                          int tabulationCount = -1)
        {
            if (attributes.IsNullOrEmpty())
                return string.Empty;

            List<string> proccessed = new();

            foreach (AttributeEntry attribute in attributes)
                proccessed.Add(GetTabulations(tabulationCount) + attribute.ToString());

            return proccessed.JoinStrings(Environment.NewLine);
        }

        private void WriteBy(object? value,
                             bool newLine,
                             int tabulationsCount,
                             bool addWhitespace)
        {
            if (!onLineTabulationsAdded && value is not IEnumerable<AttributeEntry>)
            {
                stringResult.Append(GetTabulations(tabulationsCount));
                onLineTabulationsAdded = true;
            }

            bool toProccessString = false;
            string toAppend;
            switch (value)
            {
                case char symbol:
                    toAppend = symbol.ToString();
                    toProccessString = true;
                    break;
                case string str:
                    toAppend = str;
                    toProccessString = true;
                    break;
                case Enum enumValue:
                    toAppend = ProccessEnum(enumValue);
                    toProccessString = true;
                    break;
                case Type type:
                    toAppend = type.GetProccessedName();
                    toProccessString = true;
                    break;
                case IEnumerable<AttributeEntry> attributes:
                    toAppend = ProccessAttributes(attributes, tabulationsCount);
                    break;
                default:
                    toAppend = ProccessValue(value);
                    toProccessString = true;
                    break;
            }

            if (toAppend.IsNullOrEmpty())
                return;

            if (toProccessString)
                toAppend = ProccessString(toAppend, addWhitespace);

            if (newLine)
            {
                stringResult.AppendLine(toAppend);
                onLineTabulationsAdded = false;
            }
            else
                stringResult.Append(toAppend);
        }
    }
}