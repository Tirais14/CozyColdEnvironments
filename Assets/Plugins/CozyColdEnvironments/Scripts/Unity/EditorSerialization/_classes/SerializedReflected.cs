using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Unity.EditorSerialization
{
    public class SerializedReflected
        : Serialized<SerializedTuple<SerializedType, Reflected.Settings>, Reflected>
    {
        public SerializedReflected()
        {
        }

        public SerializedReflected(Reflected defaultValue) : base(defaultValue)
        {
        }

        protected override SerializedTuple<SerializedType, Reflected.Settings> ConvertToInput(Reflected output)
        {
            return SerializedTuple.Create(new SerializedType(), Value.settings);
        }

        protected override Reflected ConvertToOutput(SerializedTuple<SerializedType, Reflected.Settings> input)
        {
            return new Reflected(input.Value.Item1, input.Value.Item2);
        }
    }
}
