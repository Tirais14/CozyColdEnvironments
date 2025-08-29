#nullable enable
using System;
using static CozyColdEnvironments.FileSystem.ScriptUtils.Syntax;
namespace CozyColdEnvironments.FileSystem.ScriptUtils
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
