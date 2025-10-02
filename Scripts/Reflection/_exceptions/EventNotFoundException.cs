#nullable enable
using System;
using System.Reflection;

namespace CCEnvs.Reflection
{
    public class EventNotFoundException : MemberNotFoundException
    {
        public EventNotFoundException()
        {
        }

        public EventNotFoundException(Type reflectedType,
                                      string? name = null,
                                      BindingFlags? bindingFlags = null)
            :
            base(reflectedType, name, bindingFlags)
        {
        }
    }
}
