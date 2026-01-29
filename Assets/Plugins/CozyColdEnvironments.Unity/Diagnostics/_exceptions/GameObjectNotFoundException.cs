using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable

namespace CCEnvs.Unity.Diagnostics
{
    public sealed class GameObjectNotFoundException : CCException
    {
        public GameObjectNotFoundException(object? key = null, Exception? innerException = null) 
            :
            base(Sentence.Empty.Add($"{nameof(GameObject)}...")
                .AddIfNotDefault(key)
                .Continue()
                .Add("not found")
                .ToString(),
                innerException)
        {
        }

        public static void ThrowIfNull([NotNull] GameObject? obj, object? key)
        {
            if (obj.IsDefault())
                throw new GameObjectNotFoundException();
        }
    }
}