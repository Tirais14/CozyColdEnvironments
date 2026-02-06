#nullable enable
namespace CCEnvs.Unity.Components
{
    public interface IGameObjectExtraInfo
    {
        public string? PersistenGuid { get; }
        public string? RuntimeId { get; }
        public HierarchyPath HierarchyPath { get; }
    }
}
