using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public class GameObjectQueryException : CCException
    {
        public GameObjectQueryException(GameObject? target,
            GameObjectQuery.Settings settings = GameObjectQuery.Settings.None,
            StringMatchSettings nameFilterSettings = StringMatchSettings.None,
            FindMode findMode = FindMode.None,
            Type? seekingComponentType = null,
            string? name = null,
            string? tag = null,
            int? layer = null,
            Type? componentFilter = null,
            string? message = null)
            :
            base(new ExceptionMessageBuilder(null)
                .AddMessage(message)
                .AddProperty(nameof(target), target.Maybe().Map(static target => target.name).GetValue())
                .AddProperty(nameof(seekingComponentType), seekingComponentType)
                .AddProperty(nameof(settings), settings)
                .AddProperty(nameof(findMode), findMode)
                .AddProperty(nameof(name), name)
                .AddProperty(nameof(nameFilterSettings), nameFilterSettings)
                .AddProperty(nameof(tag), tag)
                .AddProperty(nameof(layer), layer)
                .AddProperty(nameof(componentFilter), componentFilter)
                .ToStringAndDispose()
                )
        {
        }
    }
}
