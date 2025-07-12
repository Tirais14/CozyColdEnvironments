using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib
{
    public readonly struct ResolvedMemberInvokable
    {
        public readonly MemberInfo member;
        private readonly object[] args;

        public IReadOnlyList<object> Args => args;

        public ResolvedMemberInvokable(MemberInfo member, object[]? args = null)
        {
            this.member = member;
            this.args = args ?? Array.Empty<object>();
        }

        public readonly object? Invoke(object? invokableObject = null, BindingFlags? bindingFlags = null,
            Binder? binder = null, CultureInfo? culture = null)
        {
            if (member is MethodInfo method)
            {
                if (invokableObject.IsNull())
                {
                    throw new ArgumentException("Method requires invokable object.");
                }

                if (bindingFlags.HasValue || binder is not null || culture is not null)
                {
                    return method.Invoke(invokableObject, bindingFlags ?? BindingFlags.Default, binder, args, culture);
                }
                else
                {
                    return method.Invoke(invokableObject, args);
                }
            }
            else if (member is ConstructorInfo constructor)
            {
                if (bindingFlags.HasValue || binder is not null || culture is not null)
                {
                    return constructor.Invoke(invokableObject, bindingFlags ?? BindingFlags.Default,
                        binder, args, culture);
                }
                else
                {
                    return constructor.Invoke(args);
                }
            }

            return null;
        }
    }
}