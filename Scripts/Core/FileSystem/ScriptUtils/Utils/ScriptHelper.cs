using System;

#nullable enable 
namespace UTIRLib.FileSystem.ScriptUtils
{
    public static class ScriptHelper
    {
        public static NamespaceEntry WrapToNamespace(NamespaceEntry ns, params IType[] types)
        {
            if (types is null)
                throw new ArgumentNullException(nameof(types));
            if (types.IsEmpty())
                return ns with { };

            return ns with { Content = types };
        }

        //public static ITypeMember[] OrderTypeMembers(ITypeMember[] typeMembers)
        //{
        //    var ordered = new ITypeMember[typeMembers.Length];

        //    IField[] fields = typeMembers.ExtractFields<IField>();
        //    IMethod[] methods = typeMembers.ExtractMethods<IMethod>();

        //    fields.CopyTo(ordered, 0);
        //    methods.CopyTo(ordered, fields.Length);

        //    return ordered;
        //}
    }
}
