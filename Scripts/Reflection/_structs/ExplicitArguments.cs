using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#nullable enable
namespace CCEnvs.Reflection.Data
{
    public struct ExplicitArguments
        : IReadOnlyList<ExplicitArgument>,
        IEquatable<ExplicitArguments>
    {
        public static ExplicitArguments Empty => new(Array.Empty<ExplicitArgument>());
        public static ExplicitArguments EmptyIgnoreOptional => new(Array.Empty<ExplicitArgument>())
        {
            IgnoreOptionalArguments = true
        };

        public readonly int Count => Arguments.Count;
        public readonly ReadOnlyCollection<ExplicitArgument> Arguments { get; }
        public readonly ExplicitArgument this[int index] => Arguments[index];
        public bool IgnoreOptionalArguments { readonly get; set; }

        public readonly CCParameters RequiredArguments => new(Arguments.Select(x => x.Parameter)
                                                              .Where(x => !x.HasDefaultValue)
                                                              .ToArray());

        public ExplicitArguments(params ExplicitArgument[] args) : this()
        {
            Arguments = new ReadOnlyCollection<ExplicitArgument>(args);
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

        public readonly Type[] GetTypes()
        {
            return ((CCParameters)this).Select(x => x.ParameterType).ToArray();
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

        public readonly IEnumerator<ExplicitArgument> GetEnumerator()
        {
            return Arguments.GetEnumerator();
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
