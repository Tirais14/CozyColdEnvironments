#nullable enable
using System;
using CCEnvs.FuncLanguage;

namespace CCEnvs.Patterns.Commands
{
    public readonly struct CommandSignature : IEquatable<CommandSignature>
    {
        public Maybe<Type> CommandType { get; }

        public string CommandName { get; }

        public Identifier ID { get; }

        public CommandSignature(
            Type? commandType = null,
            string? commandName = null,
            Identifier id = default)
        {
            CommandType = commandType;
            CommandName = commandName ?? string.Empty;
            ID = id;
        }

        public static bool operator ==(CommandSignature left, CommandSignature right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CommandSignature left, CommandSignature right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{nameof(CommandType)}: {CommandType}; {CommandName}: {CommandName}.";
        }

        public bool Equals(CommandSignature other)
        {
            return CommandType.Equals(other.CommandType)
                   &&
                   CommandName == other.CommandName
                   &&
                   ID == other.ID;
        }

        public override bool Equals(object? obj)
        {
            return obj is CommandSignature info && Equals(info);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CommandType, CommandName, ID);
        }
    }
}
