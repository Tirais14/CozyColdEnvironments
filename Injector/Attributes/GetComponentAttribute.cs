using System;

#nullable enable

namespace UTIRLib.Injector
{
    /// <summary>
    /// Same as the GetComponent, but more flexible and readable.
    /// Default only works with <see cref="MonoX"/>,
    /// but for other objects can be used <see cref="ComponentInjector.Inject(UnityEngine.Component)"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetComponentAttribute : Attribute
    {
        public GetComponentSettings Settings { get; set; }

        public GetComponentAttribute(GetComponentSettings settings = GetComponentSettings.GetComponent) =>
            Settings = settings;
    }
}