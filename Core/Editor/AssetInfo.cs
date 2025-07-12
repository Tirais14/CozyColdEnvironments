using UnityEngine;

#nullable enable

namespace UTIRLib.Editor
{
    public class AssetInfo<T> where T : Object
    {
        public T Object { get; private set; }
        public string Path { get; private set; }
        public string Guid { get; private set; }

        public AssetInfo(T obj, string path)
        {
            Object = obj;
            Path = path;
        }

        public AssetInfo(T obj, string path, string guid)
        {
            Object = obj;
            Path = path;
            Guid = guid;
        }
    }
}