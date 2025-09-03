#nullable enable
using System;
using System.Linq;

namespace CCEnvs.Reflection.Data
{
    public readonly struct ExplicitArgument : IEquatable<ExplicitArgument>
    {
        public CCParameterInfo Parameter { get; }
        public object? Value { get; }

        public ExplicitArgument(CCParameterInfo parameter,
                                object? value)
        {
            Parameter = parameter;
            this.Value = value;
        }

        public ExplicitArgument(object value)
            :
            this(new CCParameterInfo(value.GetType(),
                hasDefaultValue: false,
                hasModifier: false),
                value)
        {
        }
        public ExplicitArgument(CCParameterInfo parameter)
            :
            this(parameter, parameter.HasDefaultValue ? Type.Missing : null)
        {
        }

        public static ExplicitArgument Create<T>(T? value)
        {
            return new ExplicitArgument(new CCParameterInfo(typeof(T),
                hasDefaultValue: false,
                hasModifier: false),
                value);
        }

        public static bool operator ==(ExplicitArgument left, ExplicitArgument right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ExplicitArgument left, ExplicitArgument right)
        {
            return !left.Equals(right);
        }

        public static explicit operator TypeValuePair(ExplicitArgument arg)
        {
            return arg.ToTypeValuePair();
        }

        public TypeValuePair ToTypeValuePair() => new(Parameter.ParameterType, Value);

        public bool Equals(ExplicitArgument other)
        {
            return Parameter == other.Parameter
                   &&
                   Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is ExplicitArgument typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Parameter, Value);
        }
    }

    public static class ExplicitArgumentExtensions
    {
        /// <exception cref="ArgumentNullException"></exception>
        public static Arguments AsArguments(this ExplicitArgument[] args)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            return new Arguments(args.Select(x => x.Value).ToArray());
        }
    }
}
