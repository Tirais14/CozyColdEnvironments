using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.UnityX.EditorSerialization;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.UnityX.Components
{
    [DisallowMultipleComponent]
    public sealed class PersistentID : CCBehaviour
    {
#if UNITY_EDITOR
        private static int usedExpirationKey;

        private static bool isUsedExpired => usedExpirationKey != UnityEditorDomain.PlayModeEntranceCount;
#endif

        private static readonly HashSet<int> used = new();

        [SerializeField]
        private int value;

        public int Value {
            get => value;
            set
            {
                Unregister(this.value);

                if (value != 0)
                {
                    Register(value);
                    this.value = value;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Value = GetNextID();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Unregister(Value);
        }

        public static bool TryFind(int id, [NotNullWhen(true)] out PersistentID? result, Scene scene = default)
        {
            foreach (var persistentID in GameObjectQuery.World.OnScene(scene).IncludeInactive().Components<PersistentID>())
            {
                if (persistentID.Value == id)
                {
                    result = persistentID;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public static int GetNextID()
        {
            int id;
            do
            {
                id = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }
            while (id == 0 || used.Contains(id));

            return id;
        }

        public static bool Unregister(int id) => used.Remove(id);

        public static void Register(int id)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (isUsedExpired)
                    foreach (var persistentID in GameObjectQuery.World.Components<PersistentID>())
                        if (persistentID.value != 0)
                            used.Add(persistentID.value);

                usedExpirationKey = UnityEditorDomain.PlayModeEntranceCount;
            }
#endif

            ValidateID(id);
            used.Add(id);
        }

        private static void ValidateID(int id)
        {
            if (used.Contains(id))
            {
                throw new InvalidOperationException(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Id already used")
                    .AddProperty("ID", id)
                    .ToStringAndDispose()
                    );
            }
        }

        [OnInstallExecutable]
        private static void OnInstall()
        {
            used.Clear();
        }
    }
}
