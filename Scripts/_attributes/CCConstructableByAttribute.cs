using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

#nullable enable
namespace CCEnvs.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true)]
    public class CCConstructableByAttribute : Attribute, ICCAttribute
    {
        public Type StaticType { get; }
        public string MethodName { get; }
        public ReadOnlyCollection<CCParameterInfo> Parameters { get; }
        public Type[] GenericArguments { get; }

        public Reflected Reflected => new(StaticType);

        public CCConstructableByAttribute(Type staticType,
                                          string methodName = ".ctor",
                                          ReadOnlyCollection<CCParameterInfo>? parameters = null,
                                          Type[]? genericArguments = null)
        {
            Validate.ArgumentNull(staticType, nameof(staticType));
            Validate.StringArgument(methodName, nameof(methodName));

            StaticType = staticType;
            MethodName = methodName;
            Parameters = parameters ?? new ReadOnlyCollection<CCParameterInfo>(Array.Empty<CCParameterInfo>());
            GenericArguments = genericArguments ?? Type.EmptyTypes;
        }
    }
}
