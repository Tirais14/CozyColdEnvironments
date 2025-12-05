using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public static class GameObjectExtensions
    {
        public static RectTransform RectTransform(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            return source.transform.To<RectTransform>();
        }
        public static RectTransform RectTransform(this Component source)
        {
            return source.gameObject.RectTransform();
        }

        /// <exception cref="System.ArgumentNullException"></exception>
        public static void ApplySettings(this GameObject value, GameObjectSettings settings)
        {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            if (settings == null)
                throw new System.ArgumentNullException(nameof(settings));

            settings.ApplyTo(value);
        }

        /// <returns>LINQ Enumerator</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Component> ComponentsNonAlloc(this GameObject source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            int count = source.GetComponentCount();
            for (int i = 0; i < count; i++)
                yield return source.GetComponentAtIndex(i);
        }
    }
}