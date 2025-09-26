using CCEnvs.Diagnostics;
using System.Reflection;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    public sealed class FieldReference : TypeMemberReference<FieldInfo>
    {
        public FieldReference()
        {
        }

        public FieldReference(FieldInfo defaultValue)
            :
            base(defaultValue)
        {
        }

        protected override FieldInfo ConvertToOutput(
            SerializedTuple<SerializedType, string, MemberBindings> input)
        {
            return input.item1.Value.GetField(input.item2, input.item3.Unfold())
                   ?? 
                   throw new FieldNotFoundException(input.item1, input.item2, input.item3.Unfold());
        }
    }
}
