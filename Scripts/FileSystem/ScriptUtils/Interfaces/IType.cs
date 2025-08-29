#nullable enable
using System;
using static CCEnvs.FileSystem.ScriptUtils.Syntax;
namespace CCEnvs.FileSystem.ScriptUtils
{
    public interface IType :
        IScriptContent,
        IAttributesProvider, 
        IUsingsProvider,
        IContentProvider,
        IAccessModifierProvider
    {
        OtherModifiers OtherModifiers { get; set; }
        DataType DataType { get; }
        string TypeName { get; set; }
        Type[] ParentTypes { get; set; }
        IScriptContent[] Members { get; set; }
    }
}
