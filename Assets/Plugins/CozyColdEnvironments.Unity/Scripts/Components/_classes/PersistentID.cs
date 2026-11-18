using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.UnityX.EditorSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Components
{
    [DisallowMultipleComponent]
    public sealed class PersistentID : CCBehaviour
    {
        private static readonly HashSet<int> used = new();

        [SerializeField]
        private SerializedNullable<int> value;

        public int Value {
            get => value.Data ?? 0;
            private set => this.value = value;
        }

        protected override void Awake()
        {
            base.Awake();

            if (value.Data is null)
                value = GetNextID();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Unregister(Value);
        }

        public static bool Unregister(int id) => used.Remove(id);

        public static void Register(int id)
        {
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

        public void SetID(int id)
        {
            ValidateID(id);
            used.Add(id);
            Unregister(Value);
            Value = id;
        }

        private int GetNextID()
        {
            int id;
            do
            {
                id = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }
            while (used.Contains(id));

            return id;
        }
    }
}
