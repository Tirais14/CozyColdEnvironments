using System;
using System.Reflection;
using UnityEngine;

#nullable enable
namespace CozyColdEnvironments.Unity.Editor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DropdownMenuAttribute : PropertyAttribute
    {
        private readonly Type menuItemsGetterType;
        private readonly string menuItemsGetterName;

        private readonly Type? argumentsGetterType;
        private readonly string? argumentsGetterName;

        public MethodInfo MenuItemsGetter {
            get
            {
                MethodInfo method = menuItemsGetterType.GetMethod(menuItemsGetterName,
                                    BindingFlagsDefault.All);

                return method;
            }
        }
        public MethodInfo? ArgumentsGetter {
            get
            {
                if (!HasArgumentsGetter)
                    return null;

                MethodInfo method = argumentsGetterType!.GetMethod(argumentsGetterName,
                    BindingFlagsDefault.All);

                return method;
            }
        }
        public bool HasArgumentsGetter => argumentsGetterType != null
                                          && 
                                          argumentsGetterName.IsNotNullOrWhiteSpace();

        /// <summary>
        /// Allowed only static methods. They must be return arrays. First <see cref="string"/>[]. Second <see cref="object"/>[]
        /// </summary>
        /// <param name="menuItemsGetterType">Type in which the getter declared</param>
        /// <param name="menuItemsGetterName">Allowed only static methods</param>
        public DropdownMenuAttribute(Type menuItemsGetterType, string menuItemsGetterName)
        {
            this.menuItemsGetterType = menuItemsGetterType;
            this.menuItemsGetterName = menuItemsGetterName;
        }

        /// <summary>
        /// Allowed only static methods. They must be return arrays. First <see cref="string"/>[]. Second <see cref="object"/>[]
        /// </summary>
        /// <param name="menuItemsGetterType">Type in which the getter declared</param>
        /// <param name="menuItemsGetterName">Allowed only static methods</param>
        /// <param name="argumentsGetterType"></param>
        /// <param name="argumentsGetterName"></param>
        public DropdownMenuAttribute(Type menuItemsGetterType,
                                     string menuItemsGetterName,
                                     Type argumentsGetterType,
                                     string argumentsGetterName) : this(menuItemsGetterType,
                                                                       menuItemsGetterName)
        {
            this.argumentsGetterType = argumentsGetterType;
            this.argumentsGetterName = argumentsGetterName;
        }
    }
}
