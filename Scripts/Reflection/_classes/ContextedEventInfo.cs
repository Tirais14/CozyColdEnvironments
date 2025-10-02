#nullable enable
using CCEnvs.Attributes;
using CCEnvs.Reflection;
using System;
using System.Reflection;

namespace CCEnvs
{
    public class ContextedEventInfo<T> : ContextedMemberInfo
        where T : Delegate
    {
        protected EventInfo value;

        public ContextedEventInfo(object? context, EventInfo eventInfo) : base(context)
        {
        }

        public static implicit operator ContextedEventInfo(ContextedEventInfo<T> source)
        {
            return new ContextedEventInfo(source.context, source.value);
        }

        public void AddEventHandler(Delegate func)
        {
            value.AddEventHandler(context, func);
        }
        public void AddEventHanlder(T func)
        {
            AddEventHandler(func);
        }

        public void RemoveEventHandler(Delegate func)
        {
            value.RemoveEventHandler(context, func);
        }
        public void RemoveEventHanlder(T func)
        {
            RemoveEventHandler(func);
        }
    }
    public class ContextedEventInfo : ContextedEventInfo<Delegate>
    {
        public ContextedEventInfo(object? context, EventInfo eventInfo) 
            : 
            base(context, eventInfo)
        {
        }

        [Converter]
        public ContextedEventInfo<T> Convert<T>()
            where T : Delegate
        {
            return new ContextedEventInfo<T>(context, value);
        }
    }
}
