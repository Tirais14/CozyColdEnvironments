using CCEnvs.Reflection;
using CCEnvs.Snapshots;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class SnapshotConverter : PolymorphJsonConverter<ISnapshot>
    {
    }
}
