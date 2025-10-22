using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using System;
using UnityEngine;

#nullable enable

namespace CCEnvs.Unity.Diagnostics
{
    public class ComponentNotFoundException : CCException
    {
        public ComponentNotFoundException(Type? componentType = null,
                                          GameObject? context = null,
                                          Exception? innerException = null)
            : base(Sentence.Empty.Add("Component...")
                  .AddIfNotDefault(() => componentType!.GetFullName(), componentType)
                  .Continue()
                  .Add("not found...")
                  .AddIfNotDefault($"in {nameof(GameObject)}: {context!.name}", context)
                  .ToString(),
                  innerException)
        {
        }

        public static void ThrowIfNull(Component? obj,
                                       Type? componentType,
                                       GameObject? context)
        {
            if (obj.IsDefault())
                throw new ComponentNotFoundException(componentType, context);
        }
    }
}