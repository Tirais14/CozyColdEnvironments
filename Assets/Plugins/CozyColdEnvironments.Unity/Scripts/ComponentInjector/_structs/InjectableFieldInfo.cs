using System.Reflection;

#nullable enable
namespace CCEnvs.UnityX.ComponentInjections
{
    public readonly struct InjectableFieldInfo
    {
        public FieldInfo Field { get; }

        public GetComponentAttribute Attribute { get; }

        public InjectableFieldInfo(FieldInfo field, GetComponentAttribute attribute)
        {
            Field = field;
            Attribute = attribute;
        }

        public void Deconstruct(out FieldInfo field, out GetComponentAttribute attribute)
        {
            field = Field;
            attribute = Attribute;
        }
    }
}
