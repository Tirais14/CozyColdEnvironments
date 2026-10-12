using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.EditorSerialization
{
    public abstract class ScriptableEditorSerialized<T> : ScriptableObject, IEditorSerialized<T>
    {
        [NonSerialized]
        private T? data;

        [NonSerialized]
        private bool isDataCreated;

#if UNITY_EDITOR
        [NonSerialized]
        private int dataCreationKey;
#endif

        public T Data {
            get
            {
#if UNITY_EDITOR
                if (isDataCreated &&
                    dataCreationKey != UnityEditorDomain.PlayModeEntranceCount
                    )
                    ResetData();
#endif


                if (!isDataCreated)
                {
                    data = CreateData();
#if UNITY_EDITOR
                    dataCreationKey = UnityEditorDomain.PlayModeEntranceCount;
#endif
                    isDataCreated = true;
                }

                return data!;
            }
        }

        public abstract T CreateData();

        public void ResetData()
        {
            data = default;
            isDataCreated = false;
        }
    }
}
