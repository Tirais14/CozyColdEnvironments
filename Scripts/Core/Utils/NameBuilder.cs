using System;
using System.Collections.Generic;
using System.Linq;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.Utils
{
    public class NameBuilder
    {
        public static string Separator { get; set; } = "_";

        //private readonly Dictionary<int, string> parts;
        //private int? smallerIndex;
        //private int? biggerIndex;
        //private int lastAdded;

        //public string? CustomSeparator { get; }

        //public string this[int index] => parts[index];
        //public string FirstWord {
        //    get {
        //        if (!smallerIndex.HasValue)
        //            return string.Empty;

        //        return parts[smallerIndex.Value];
        //    }
        //}
        //public string LastWord {
        //    get {
        //        if (!biggerIndex.HasValue)
        //            return string.Empty;

        //        return parts[biggerIndex.Value];
        //    }
        //}

        //public NameBuilder(NameBuilder original)
        //{
        //    parts = new Dictionary<int, string>(original.parts);

        //    CustomSeparator = original.CustomSeparator;
        //}

        //public NameBuilder(string? customSeparator, params KeyValuePair<int, string>[] parts)
        //{
        //    this.parts = new Dictionary<int, string>();

        //    CustomSeparator = customSeparator;

        //    this.parts.AddRange(parts);
        //}

        //public NameBuilder(params KeyValuePair<int, string>[] parts) : this(customSeparator: null, parts)
        //{
        //}

        //public NameBuilder(string? customSeparator,
        //                   params (int pos, string value)[] parts): this(customSeparator,
        //                                                                 parts.ToArray())
        //{
        //}

        //public NameBuilder(params (int pos, string value)[] parts) : this(customSeparator: null, parts)
        //{
        //}

        /// <exception cref="ArgumentNullException"></exception>
        public static string Join(string name, string? prefix = null, string? postfix = null)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (prefix.IsNotNullOrEmpty())
            {
                if (!prefix.EndsWith(Separator))
                {
                    prefix = ToPrefix(prefix);
                }

                name = prefix + name;
            }

            if (postfix.IsNotNullOrEmpty())
            {
                if (!postfix.StartsWith(Separator))
                {
                    postfix = ToPostfix(postfix);
                }

                name += postfix;
            }

            return name;
        }

        public static string Combine(params string[] nameParts)
        {
            if (nameParts.IsEmpty())
            {
                return string.Empty;
            }

            return string.Join(Separator, nameParts);
        }

        /// <exception cref="StringArgumentException"></exception>
        public static string GetPrefix(string name)
        {
            if (name.IsNullOrEmpty())
            {
                throw new StringArgumentException(nameof(name), name);
            }

            string[] splitted = name.Split(Separator);

            if (splitted.Length <= 1)
            {
                return string.Empty;
            }
            else return splitted[0];
        }

        public static string RemovePrefix(string name) => name.Delete(GetPrefix(name));

        /// <exception cref="StringArgumentException"></exception>
        public static string GetPostfix(string name)
        {
            if (name.IsNullOrEmpty())
            {
                throw new StringArgumentException(nameof(name), name);
            }

            string[] splitted = name.Split(Separator);

            if (splitted.Length <= 1)
            {
                return string.Empty;
            }
            else return splitted[^1];
        }

        /// <exception cref="StringArgumentException"></exception>
        public static string RemovePostfix(string? name)
        {
            if (name.IsNullOrEmpty())
            {
                throw new StringArgumentException(nameof(name), name);
            }

            return name.Delete(GetPostfix(name));
        }

        /// <exception cref="WrongStringException"></exception>
        public static string GetWord(string name)
        {
            if (name.IsNullOrEmpty())
            {
                throw new StringArgumentException(nameof(name), name);
            }

            string[] splitted = name.Split(Separator);

            if (splitted.Length >= 3)
            {
                return splitted[1..^1].JoinStrings(Separator);
            }
            else return RemovePrefix(name);
        }

        /// <exception cref="WrongStringException"></exception>
        public static string ToPrefix(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new StringArgumentException(nameof(str), str);
            }

            return str + Separator;
        }

        /// <exception cref="WrongStringException"></exception>
        public static string ToPostfix(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new StringArgumentException(nameof(str), str);
            }

            return Separator + str;
        }

        //public NameBuilder With(string? customSeparator)
        //{
        //    return new NameBuilder(customSeparator, parts.ToArray());
        //}
        //public NameBuilder With(params KeyValuePair<int, string>[] parts)
        //{
        //    NameBuilder copy = new(CustomSeparator, this.parts.ToArray());

        //    foreach (var part in parts)
        //        if (!copy.parts.TrySetValue(part))
        //            copy.parts.Add(part);
        //}
    }
}