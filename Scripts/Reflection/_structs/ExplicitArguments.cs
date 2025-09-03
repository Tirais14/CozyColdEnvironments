using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#nullable enable
namespace CCEnvs.Reflection.Data
{
    public readonly struct ExplicitArguments
        : IReadOnlyList<ExplicitArgument>,
        IEquatable<ExplicitArguments>
    {
        public static ExplicitArguments Empty => new(Array.Empty<object>());
        /// <summary>
        /// A constructor that contains only optional parameters is also considered empty
        /// </summary>
        public static ExplicitArguments OptionalAsEmpty => new(new ExplicitArgument());

        public int Count => Arguments.Count;
        public ReadOnlyCollection<ExplicitArgument> Arguments { get; }
        public ExplicitArgument this[int index] => Arguments[index];

        public ExplicitArguments(params ExplicitArgument[] args)
        {
            Arguments = new ReadOnlyCollection<ExplicitArgument>(args);
        }

        public ExplicitArguments(params object[] args)
            :
            this(args.Select(x => new ExplicitArgument(new CCParameterInfo(x.GetType()),
                                                       x)).ToArray())
        {
        }

        public ExplicitArguments(object args, bool singleArg)
            :
            this(args)
        {
        }

        public static bool operator ==(ExplicitArguments left, ExplicitArguments right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ExplicitArguments left, ExplicitArguments right)
        {
            return !left.Equals(right);
        }

        public static explicit operator CCParameters(ExplicitArguments args)
        {
            return new CCParameters(args.Select(x => x.Parameter).ToArray());
        }

        public static explicit operator object?[](ExplicitArguments args)
        {
            return args.Arguments.Select(x => x.Value).ToArray();
        }

        public static explicit operator TypeValuePair[](ExplicitArguments args)
        {
            return args.ToTypeValuePairs();
        }

        public readonly TypeValuePair[] ToTypeValuePairs()
        {
            return Arguments.Select(x => new TypeValuePair(x.Parameter.ParameterType, x.Value)).ToArray();
        }

        public readonly bool Equals(ExplicitArguments other)
        {
            return Arguments.SequenceEqual(other.Arguments);
        }
        public readonly override bool Equals(object obj)
        {
            return obj is ExplicitArguments typed && Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Arguments);
        }

        public readonly override string ToString()
        {
            if (Arguments.IsEmpty())
                return "empty";

            TypeValuePair[] typeValuePairs = ToTypeValuePairs();
            var builder = new StringBuilder();
            TypeValuePair pair;
            for (int i = 0; i < typeValuePairs.Length; i++)
            {
                pair = typeValuePairs[i];
                builder.Append($"position = {i}, type = {pair.Type.GetName()}, value = {pair.Value}; ");
            }

            return builder.ToString();
        }

        public IEnumerator<ExplicitArgument> GetEnumerator()
        {
            return Arguments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
