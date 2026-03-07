using System;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public class GameObjectQueryException : CCException
    {
        public GameObjectQueryException(GameObject? target,
            GameObjectQuery.Settings settings,
            FindMode findMode,
            Type? seekingComponentType = null,
            string? name = null,
            string? tag = null,
            int layer = -1,
            Type? componentFilter = null,
            string? message = null)
            :
            base(Sentence.Empty
                    .AddIfNotDefault(() => $"{message}...", message)
                    .AddIfNotDefault(() => $"Target: {target!.name},...", target)
                    .Add($"Settings: {settings},...")
                    .Add($"Find Mode: {findMode},...")
                    .AddIfNotDefault(() => $"Seeking component type: {seekingComponentType.GetFullName()}", seekingComponentType)
                    .AddIfNotDefault(() => $"Name: {name},...", name)
                    .AddIfNotDefault(() => $"Tag: {tag}m...", tag)
                    .Add($"Layer: {layer},...", _ => layer > -1)
                    .AddIfNotDefault(() => $"Must contain type: {componentFilter!.GetFullName()},...", componentFilter)
                    .ToString())
        {
        }
    }
}
