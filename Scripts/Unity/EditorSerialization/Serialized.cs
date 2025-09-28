using CCEnvs.Attributes;
using CCEnvs.Diagnostics;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable S3459
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public abstract class Serialized<TOutput>
        : IEditorSerialized<TOutput>,
        ITransformable<TOutput>
    {
        public virtual TOutput Value { [Converter] get; protected set; } = default!;

        protected Serialized()
        {
        }

        protected Serialized(TOutput defaultValue)
        {
            Value = defaultValue;
        }

        public static implicit operator TOutput(Serialized<TOutput> source)
        {
            return source.Value;
        }

        TOutput ITransformable<TOutput>.DoTransform() => Value;
    }
    [Serializable]
    public abstract class Serialized<TInput, TOutput>
        : Serialized<TOutput>,
        ISerializationCallbackReceiver
    {
        [SerializeField]
        protected TInput input = default!;

        private bool inputErased;

        protected virtual bool EraseInputOnSerialized => true;

        protected Serialized()
        {
        }

        protected Serialized(TOutput defaultValue)
            :
            base(defaultValue)
        {
        }


        public static implicit operator TOutput(Serialized<TInput, TOutput> source)
        {
            return source.Value;
        }

        public override string ToString()
        {
            return $"{nameof(input)}: {input} |{nameof(Value)}: {Value}";
        }

        protected abstract TOutput ConvertToOutput(TInput input);

        protected abstract TInput ConvertToInput(TOutput output);

        protected virtual void OnBeforeSerialize()
        {
        }

        protected virtual void OnAfterDeserialize()
        {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            try
            {
                if (inputErased && Value is not null)
                    input = ConvertToInput(Value);

                OnBeforeSerialize();
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            try
            {
                Value = ConvertToOutput(input);

                OnAfterDeserialize();
#if !UNITY_EDITOR
                if (EraseInputOnSerialized)
                {
                    EditorSerializing.SetDefault(ref input!);
                    inputErased = true;
                }
#endif
            }
            catch (Exception ex)
            {
                CCDebug.PrintException(ex);
            }
        }
    }
}
