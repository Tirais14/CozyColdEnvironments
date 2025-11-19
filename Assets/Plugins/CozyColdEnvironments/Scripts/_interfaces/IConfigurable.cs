#nullable enable
namespace CCEnvs
{
    public interface IConfigurable
    {
        Result<object> GetConfig(object? id = null);

        bool SetConfig(object config, object? id = null);
    }

    public interface IConfigurable<T> : IConfigurable
    {
        new Result<T> GetConfig(object? id = null);

        bool SetConfig(T config, object? id = null);

        Result<object> IConfigurable.GetConfig(object? id)
        {
            try
            {
                var x = GetConfig(id);
                return (x, null);
            }
            catch (System.Exception ex)
            {
                return (null, ex);
            }
        }

        bool IConfigurable.SetConfig(object config, object? id)
        {
            if (config is not T typed)
                return false;

            return SetConfig(typed);
        }
    }
}
