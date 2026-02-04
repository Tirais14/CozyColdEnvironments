#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace CCEnvs.Patterns.Commands
{
    public readonly struct CommandSignature : IEquatable<CommandSignature>
    {
        public Maybe<Type> CommandType { get; }
        public string CommandName { get; }

        public CommandSignature(Type? commandType = null, string? commandName = null)
        {
            CommandType = commandType;
            CommandName = commandName ?? string.Empty;
        }

        public static bool operator ==(CommandSignature left, CommandSignature right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CommandSignature left, CommandSignature right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICommandAsync? command)
        {
            if (command.IsNull())
                return false;

            return (CommandType.IsNone || command.GetType().IsType(CommandType.GetValueUnsafe()))
                   &&
                   command.Name == CommandName;
        }

        public override string ToString()
        {
            return $"{nameof(CommandType)}: {CommandType}; {CommandName}: {CommandName}.";
        }

        public bool Equals(CommandSignature other)
        {
            return CommandType.Equals(other.CommandType)
                   &&
                   CommandName == other.CommandName;
        }

        public override bool Equals(object? obj)
        {
            return obj is CommandSignature info && Equals(info);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CommandType, CommandName);
        }
    }
}
