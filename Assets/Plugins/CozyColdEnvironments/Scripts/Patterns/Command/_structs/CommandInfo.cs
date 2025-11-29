#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace CCEnvs.Patterns.Commands
{
    public readonly struct CommandInfo
    {
        public Maybe<Type> CommandType { get; }
        public string CommandName { get; }

        public CommandInfo(Type? commandType = null, string? commandName = null)
        {
            CommandType = commandType;
            CommandName = commandName ?? string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatch(ICommand? command)
        {
            if (command.IsNull())
                return false;

            return (CommandType.IsNone || command.GetType().IsType(CommandType.GetValueUnsafe()))
                   &&
                   command.CommandName == CommandName;
        }

        public override string ToString()
        {
            return $"{nameof(CommandType)}: {CommandType}; {CommandName}: {CommandName}.";
        }
    }
}
