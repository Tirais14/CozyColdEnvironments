using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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

        public int Count => Arguments.Count;
        public ReadOnlyCollection<ExplicitArgument> Arguments {
            get
            {
                arguments ??= new ReadOnlyCollection<ExplicitArgument>(
                    Array.Empty<ExplicitArgument>());

                return arguments;
            }
        }

        public ExplicitArgument this[int index] => Arguments[index];
        public bool IgnoreOptionalArguments { readonly get; set; }

        private ReadOnlyCollection<ExplicitArgument>? arguments;

        public CCParameters RequiredArguments => new(Arguments.Select(x => x.Parameter)
                                                     .Where(x => !x.HasDefaultValue)
                                                     .ToArray());

        public ExplicitArguments(params ExplicitArgument[] args) : this()
        {
            arguments = new ReadOnlyCollection<ExplicitArgument>(args);
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

        public TypeValuePair[] ToTypeValuePairs()
        {
            return Arguments.Select(x => new TypeValuePair(x.Parameter.ParameterType, x.Value)).ToArray();
        }

        public bool Equals(ExplicitArguments other)
        {
            return Arguments.SequenceEqual(other.Arguments);
        }
        public override bool Equals(object obj)
        {
            return obj is ExplicitArguments typed && Equals(typed);
        }

        public ParameterModifier GetParameterModifiers()
        {
            if (Arguments.IsEmpty())
                return default;

            var result = new ParameterModifier(Count);

            for (int i = 0; i < Arguments!.Count; i++)
            {
                if (Arguments![i].Parameter.HasModifier)
                    result[i] = true;
            }

            return result;
        }

        public readonly Type[] GetTypes()
        {
            return ((CCParameters)this).Select(x => x.ParameterType).ToArray();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Arguments);
        }

        public override string ToString()
        {
            if (Arguments.IsNullOrEmpty())
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
