#nullable enable
namespace CCEnvs.Attributes.Metadata
{
    public interface IMetdataAttribute : ICCAttribute
    {
        object Value { get; }
    }
    public interface IMetdataAttribute<out T> : IMetdataAttribute
    {
        new T Value { get; }

        object IMetdataAttribute.Value => Value!;
    }
}
