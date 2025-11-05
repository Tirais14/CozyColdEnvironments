using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
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
            (SerializedType type, string name, MemberBindings binds) tuple = input.Value;

            return tuple.type.Value.GetField(tuple.name, tuple.binds.Unfold())
                   ??
                   throw new MemberNotFoundException(
                       MemberTypes.Field,
                       reflectedType: tuple.type,
                       name: tuple.name,
                       bindingFlags: tuple.binds.Unfold()
                       );
        }
    }
}
