using CCEnvs.Diagnostics;
using System;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public class MemberNotFoundException : CCException
    {
        public MemberNotFoundException(MemberTypes memberType,
                                       Type? reflectedType = null,
                                       string? name = null,
                                       BindingFlags? bindingFlags = null,
                                       Type[]? types = null,
                                       Binder? binder = null)
            :
            base(Sentence.Empty.Add($"Member type: {memberType};...")
                .AddIfNotDefault(() => $"Reflected type: {reflectedType!.GetFullName()};...", reflectedType)
                .AddIfNotDefault(() => $"Name: {name};...", name)
                .AddIfNotDefault(() => $"BindingFlags: {bindingFlags.GetValueOrDefault()};...", bindingFlags)
                .AddIfNotDefault(() => $"Input types: {types.Select(x => x.GetFullName()).JoinStrings(", ")};...", types)
                .AddIfNotDefault(() => $"Binder: {binder};...", binder)
                .ToString())
        {

        }
    }
}
