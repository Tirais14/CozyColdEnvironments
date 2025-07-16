using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.Utils
{
    public static class ComponentHelper
    {
        /// <summary>
        /// Same as basic, but if not found component - send log
        /// </summary>
        /// <remarks>Debug:
        /// <br/><see cref="ObjectNotFoundMessage"/>
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static object? GetComponentFrom(Component source, Type toGetComponentType)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (toGetComponentType == null)
            {
                throw new ArgumentNullException(nameof(toGetComponentType));
            }

            if (!source.TryGetComponent(toGetComponentType, out var found))
            {
                return null;
            }

            return found;
        }

        /// <summary>
        /// Same as basic, but if not found component - send log
        /// </summary>
        /// <remarks>Debug:
        /// <br/><see cref="ObjectNotFoundMessage"/>
        /// </remarks>
        /// <typeparam name="TSource">May be <see langword="null"/>, but still send long</typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="source"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static TOut? GetComponentFrom<TSource, TOut>(TSource source)
            where TSource : Component
            where TOut : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.TryGetComponent<TOut>(out var found))
            {
                return null;
            }

            return found;
        }

        /// <summary>
        /// Try get or add specified <see cref="Component"/>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static T AddComponent<TSource, T>(TSource source, Type type)
            where TSource : Component
            where T : Component
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source.TryGetComponent(type, out var foundComponent))
            {
                return foundComponent.Convert<T>();
            }
            else return source.gameObject.AddComponent(type).Convert<T>();
        }

        /// <summary>
        /// Try get or add specified <see cref="Component"/>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static T AddComponent<TSource, T>(TSource source)
            where TSource : Component
            where T : Component
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source.TryGetComponent<T>(out var foundComponent))
            {
                return foundComponent;
            }
            else return source.gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Try get or add specified <see cref="Component"/>
        /// </summary>
        public static void AddComponent<TSource, T>(TSource source, [NotNull] ref T? value)
            where TSource : Component
            where T : Component => value = AddComponent<TSource, T>(source, typeof(T));
    }
}