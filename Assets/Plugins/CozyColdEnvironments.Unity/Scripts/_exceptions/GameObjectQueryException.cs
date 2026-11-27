using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX
{
    public class GameObjectQueryException : CCException
    {
        public GameObjectQueryException(string message) : base(message) { }

        public GameObjectQueryException(GameObject? target,
            GameObjectQuery.Settings settings = GameObjectQuery.Settings.None,
            StringMatchSettings nameFilterSettings = StringMatchSettings.None,
            FindMode findMode = FindMode.None,
            Type? seekingComponentType = null,
            string? name = null,
            string? tag = null,
            int? layer = null,
            Type? componentFilter = null,
            string? message = null,
            InlinedList8<(string Name, object Value)> additionalParams = default)
            :
            base(new DebugMessageBuilder(null)
                .AddMessage(message)
                .AddProperty(nameof(target), target.Maybe().Map(static target => $"{target.name}({target.GetType()})").GetValue())
                .AddProperty(nameof(seekingComponentType), seekingComponentType)
                .AddProperty(nameof(settings), settings)
                .AddProperty(nameof(findMode), findMode)
                .AddProperty(nameof(name), name)
                .AddProperty(nameof(nameFilterSettings), nameFilterSettings)
                .AddProperty(nameof(tag), tag)
                .AddProperty(nameof(layer), layer)
                .AddProperty(nameof(componentFilter), componentFilter)
                .AddPredicatedProperty(additionalParams != default && additionalParams.Length >= 1, () => additionalParams[0].Name, () => additionalParams[0].Value)
                .AddPredicatedProperty(additionalParams != default && additionalParams.Length >= 2, () => additionalParams[1].Name, () => additionalParams[1].Value)
                .AddPredicatedProperty(additionalParams != default && additionalParams.Length >= 3, () => additionalParams[2].Name, () => additionalParams[2].Value)
                .AddPredicatedProperty(additionalParams != default && additionalParams.Length >= 4, () => additionalParams[3].Name, () => additionalParams[3].Value)
                .ToStringAndDispose()
                )
        {
        }
    }
}
